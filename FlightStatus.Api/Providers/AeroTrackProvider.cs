using FlightStatus.Api.Models;
using FlightStatus.Api.Services;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Stub implementation of AeroTrack provider.
/// Provides full detail: status, scheduled & actual times, terminal, gate, delay reason.
/// Uses deterministic data to cover multiple status scenarios.
/// </summary>
public class AeroTrackProvider : IFlightStatusProvider
{
    private readonly IStatusNormaliser _normaliser;
    private readonly ILogger<AeroTrackProvider> _logger;

    public string ProviderName => "AeroTrack";

    public AeroTrackProvider(AeroTrackStatusNormaliser normaliser, ILogger<AeroTrackProvider> logger)
    {
        _normaliser = normaliser;
        _logger = logger;
    }

    private static readonly Dictionary<string, AeroTrackResponse> StubData = new()
    {
        ["SK1234"] = new AeroTrackResponse
        {
            FlightNumber = "SK1234",
            Status = "ON_SCHEDULE",
            ScheduledDeparture = new DateTime(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc),
            ActualDeparture = new DateTime(2024, 3, 15, 8, 5, 0, DateTimeKind.Utc),
            ScheduledArrival = new DateTime(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc),
            ActualArrival = new DateTime(2024, 3, 15, 11, 2, 0, DateTimeKind.Utc),
            Terminal = "T2",
            Gate = "B14",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 7, 30, 0, DateTimeKind.Utc)
        },
        ["BA456"] = new AeroTrackResponse
        {
            FlightNumber = "BA456",
            Status = "LATE",
            ScheduledDeparture = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc),
            ActualDeparture = new DateTime(2024, 3, 15, 10, 45, 0, DateTimeKind.Utc),
            ScheduledArrival = new DateTime(2024, 3, 15, 14, 0, 0, DateTimeKind.Utc),
            ActualArrival = null,
            Terminal = "T5",
            Gate = "A22",
            DelayReason = "Weather conditions at destination",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 10, 30, 0, DateTimeKind.Utc)
        },
        ["LH789"] = new AeroTrackResponse
        {
            FlightNumber = "LH789",
            Status = "NO_FLY",
            ScheduledDeparture = new DateTime(2024, 3, 15, 6, 0, 0, DateTimeKind.Utc),
            ActualDeparture = null,
            ScheduledArrival = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc),
            ActualArrival = null,
            Terminal = "T1",
            Gate = "C3",
            DelayReason = "Technical issue with aircraft",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 5, 45, 0, DateTimeKind.Utc)
        },
        ["AF101"] = new AeroTrackResponse
        {
            FlightNumber = "AF101",
            Status = "REROUTED",
            ScheduledDeparture = new DateTime(2024, 3, 15, 14, 0, 0, DateTimeKind.Utc),
            ActualDeparture = new DateTime(2024, 3, 15, 14, 10, 0, DateTimeKind.Utc),
            ScheduledArrival = new DateTime(2024, 3, 15, 17, 0, 0, DateTimeKind.Utc),
            ActualArrival = new DateTime(2024, 3, 15, 17, 30, 0, DateTimeKind.Utc),
            Terminal = "T3",
            Gate = "D7",
            DelayReason = "Diverted to alternate airport due to fog",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 17, 35, 0, DateTimeKind.Utc)
        },
        ["EK502"] = new AeroTrackResponse
        {
            FlightNumber = "EK502",
            Status = "ON_SCHEDULE",
            ScheduledDeparture = new DateTime(2024, 3, 16, 22, 0, 0, DateTimeKind.Utc),
            ActualDeparture = new DateTime(2024, 3, 16, 22, 3, 0, DateTimeKind.Utc),
            ScheduledArrival = new DateTime(2024, 3, 17, 6, 0, 0, DateTimeKind.Utc),
            ActualArrival = null,
            Terminal = "T3",
            Gate = "A1",
            LastUpdatedUtc = new DateTime(2024, 3, 16, 22, 10, 0, DateTimeKind.Utc)
        }
    };

    public Task<FlightStatusResult?> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Querying AeroTrack for {FlightNumber} on {Date}", flightNumber, date);

        if (!StubData.TryGetValue(flightNumber.ToUpperInvariant(), out var response))
        {
            _logger.LogDebug("No AeroTrack data found for {FlightNumber}", flightNumber);
            return Task.FromResult<FlightStatusResult?>(null);
        }

        var scheduledDate = DateOnly.FromDateTime(response.ScheduledDeparture);
        if (scheduledDate != date)
        {
            _logger.LogDebug("AeroTrack date mismatch for {FlightNumber}: expected {Date}, got {ScheduledDate}", flightNumber, date, scheduledDate);
            return Task.FromResult<FlightStatusResult?>(null);
        }

        var result = new FlightStatusResult
        {
            FlightNumber = response.FlightNumber,
            Date = date,
            Status = _normaliser.Normalise(response.Status),
            ScheduledDeparture = response.ScheduledDeparture,
            ActualDeparture = response.ActualDeparture,
            ScheduledArrival = response.ScheduledArrival,
            ActualArrival = response.ActualArrival,
            Terminal = response.Terminal,
            Gate = response.Gate,
            DelayReason = response.DelayReason,
            ProviderSource = ProviderName,
            LastUpdatedUtc = response.LastUpdatedUtc
        };

        _logger.LogDebug("AeroTrack returned status {Status} for {FlightNumber}", result.Status, flightNumber);
        return Task.FromResult<FlightStatusResult?>(result);
    }
}
