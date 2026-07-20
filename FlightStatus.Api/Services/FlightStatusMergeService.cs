using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Applies merge rules to select the best provider result:
/// - Both respond → prefer the one with later LastUpdatedUtc
/// - One responds → use that result  
/// - Neither responds → return Unknown with a clear message
/// After merge, applies time-based status derivation as fallback when status is Unknown.
/// </summary>
public class FlightStatusMergeService : IFlightStatusMergeService
{
    private readonly ITimeBasedStatusDerivationService _timeDerivation;
    private readonly ILogger<FlightStatusMergeService> _logger;

    public FlightStatusMergeService(
        ITimeBasedStatusDerivationService timeDerivation,
        ILogger<FlightStatusMergeService> logger)
    {
        _timeDerivation = timeDerivation;
        _logger = logger;
    }

    public FlightStatusResult Merge(IReadOnlyList<FlightStatusResult?> results, string flightNumber, DateOnly date)
    {
        var validResults = results.Where(r => r is not null).Cast<FlightStatusResult>().ToList();

        var merged = validResults.Count switch
        {
            0 => CreateUnknownResult(flightNumber, date),
            1 => validResults[0],
            _ => SelectByMostRecent(validResults)
        };

        // Apply time-based fallback if status is Unknown but we have time data
        if (merged.Status == FlightStatusCode.Unknown)
        {
            var derived = _timeDerivation.DeriveStatus(
                merged.ScheduledDeparture, merged.ActualDeparture,
                merged.ScheduledArrival, merged.ActualArrival);
            if (derived != FlightStatusCode.Unknown)
            {
                _logger.LogInformation("Applied time-based derivation for {FlightNumber}: {Status}", flightNumber, derived);
                merged.Status = derived;
            }
        }

        _logger.LogInformation("Merge result for {FlightNumber}: {Status} from {Source}",
            flightNumber, merged.Status, merged.ProviderSource);

        return merged;
    }

    private static FlightStatusResult SelectByMostRecent(List<FlightStatusResult> results)
    {
        return results
            .OrderByDescending(r => r.LastUpdatedUtc ?? DateTime.MinValue)
            .First();
    }

    private static FlightStatusResult CreateUnknownResult(string flightNumber, DateOnly date)
    {
        return new FlightStatusResult
        {
            FlightNumber = flightNumber,
            Date = date,
            Status = FlightStatusCode.Unknown,
            ProviderSource = "None",
            Message = "No flight data available from any provider"
        };
    }
}
