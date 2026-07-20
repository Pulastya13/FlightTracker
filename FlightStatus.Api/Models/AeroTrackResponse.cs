namespace FlightStatus.Api.Models;

/// <summary>
/// Response model from the AeroTrack provider.
/// Full detail: status, scheduled & actual times, terminal, gate, delay reason.
/// </summary>
public record AeroTrackResponse
{
    public required string FlightNumber { get; init; }
    public required string Status { get; init; }
    public DateTime ScheduledDeparture { get; init; }
    public DateTime? ActualDeparture { get; init; }
    public DateTime ScheduledArrival { get; init; }
    public DateTime? ActualArrival { get; init; }
    public string? Terminal { get; init; }
    public string? Gate { get; init; }
    public string? DelayReason { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}
