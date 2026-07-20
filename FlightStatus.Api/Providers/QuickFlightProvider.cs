using FlightStatus.Api.Models;
using FlightStatus.Api.Services;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Stub implementation of QuickFlight provider.
/// Provides minimal detail: status and scheduled times only.
/// Uses deterministic data to cover multiple status scenarios.
/// </summary>
public class QuickFlightProvider : IFlightStatusProvider
{
    private readonly IStatusNormaliser _normaliser;
    private readonly ILogger<QuickFlightProvider> _logger;

    public string ProviderName => "QuickFlight";

    public QuickFlightProvider(QuickFlightStatusNormaliser normaliser, ILogger<QuickFlightProvider> logger)
    {
        _normaliser = normaliser;
        _logger = logger;
    }

    private static readonly Dictionary<string, QuickFlightResponse> StubData = new()
    {
        ["SK1234"] = new QuickFlightResponse
        {
            FlightCode = "SK1234",
            CurrentStatus = "punctual",
            DepartureScheduled = new DateTime(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc),
            ArrivalScheduled = new DateTime(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc),
            LastUpdatedUtc = new DateTime(2024, 3, 15, 7, 0, 0, DateTimeKind.Utc)
        },
        ["BA456"] = new QuickFlightResponse
        {
            FlightCode = "BA456",
            CurrentStatus = "behind_schedule",
            DepartureScheduled = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc),
            ArrivalScheduled = new DateTime(2024, 3, 15, 14, 0, 0, DateTimeKind.Utc),
            LastUpdatedUtc = new DateTime(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc)
        },
        ["LH789"] = new QuickFlightResponse
        {
            FlightCode = "LH789",
            CurrentStatus = "cancelled",
            DepartureScheduled = new DateTime(2024, 3, 15, 6, 0, 0, DateTimeKind.Utc),
            ArrivalScheduled = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc),
            LastUpdatedUtc = new DateTime(2024, 3, 15, 5, 30, 0, DateTimeKind.Utc)
        },
        ["UA303"] = new QuickFlightResponse
        {
            FlightCode = "UA303",
            CurrentStatus = "diverted",
            DepartureScheduled = new DateTime(2024, 3, 15, 16, 0, 0, DateTimeKind.Utc),
            ArrivalScheduled = new DateTime(2024, 3, 15, 20, 0, 0, DateTimeKind.Utc),
            LastUpdatedUtc = new DateTime(2024, 3, 15, 19, 45, 0, DateTimeKind.Utc)
        },
        ["EK502"] = new QuickFlightResponse
        {
            FlightCode = "EK502",
            CurrentStatus = "punctual",
            DepartureScheduled = new DateTime(2024, 3, 16, 22, 0, 0, DateTimeKind.Utc),
            ArrivalScheduled = new DateTime(2024, 3, 17, 6, 0, 0, DateTimeKind.Utc),
            LastUpdatedUtc = new DateTime(2024, 3, 16, 23, 0, 0, DateTimeKind.Utc)
        }
    };

    public Task<FlightStatusResult?> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Querying QuickFlight for {FlightNumber} on {Date}", flightNumber, date);

        if (!StubData.TryGetValue(flightNumber.ToUpperInvariant(), out var response))
        {
            _logger.LogDebug("No QuickFlight data found for {FlightNumber}", flightNumber);
            return Task.FromResult<FlightStatusResult?>(null);
        }

        var scheduledDate = DateOnly.FromDateTime(response.DepartureScheduled);
        if (scheduledDate != date)
        {
            _logger.LogDebug("QuickFlight date mismatch for {FlightNumber}: expected {Date}, got {ScheduledDate}", flightNumber, date, scheduledDate);
            return Task.FromResult<FlightStatusResult?>(null);
        }

        var result = new FlightStatusResult
        {
            FlightNumber = response.FlightCode,
            Date = date,
            Status = _normaliser.Normalise(response.CurrentStatus),
            ScheduledDeparture = response.DepartureScheduled,
            ActualDeparture = null,
            ScheduledArrival = response.ArrivalScheduled,
            ActualArrival = null,
            Terminal = null,
            Gate = null,
            DelayReason = null,
            ProviderSource = ProviderName,
            LastUpdatedUtc = response.LastUpdatedUtc
        };

        _logger.LogDebug("QuickFlight returned status {Status} for {FlightNumber}", result.Status, flightNumber);
        return Task.FromResult<FlightStatusResult?>(result);
    }
}
