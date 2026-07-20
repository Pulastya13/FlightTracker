namespace FlightStatus.Api.Models;

/// <summary>
/// Response model from the QuickFlight provider.
/// Minimal detail: status and scheduled times only.
/// </summary>
public record QuickFlightResponse
{
    public required string FlightCode { get; init; }
    public required string CurrentStatus { get; init; }
    public DateTime DepartureScheduled { get; init; }
    public DateTime ArrivalScheduled { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}
