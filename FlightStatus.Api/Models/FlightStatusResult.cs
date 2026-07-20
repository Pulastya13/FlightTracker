namespace FlightStatus.Api.Models;

/// <summary>
/// Unified flight status result returned by the API.
/// Merges and normalises data from all providers.
/// </summary>
public record FlightStatusResult
{
    public required string FlightNumber { get; init; }
    public required DateOnly Date { get; init; }
    public FlightStatusCode Status { get; set; }
    public DateTime? ScheduledDeparture { get; init; }
    public DateTime? ActualDeparture { get; init; }
    public DateTime? ScheduledArrival { get; init; }
    public DateTime? ActualArrival { get; init; }
    public string? Terminal { get; init; }
    public string? Gate { get; init; }
    public string? DelayReason { get; init; }
    public required string ProviderSource { get; init; }
    public DateTime? LastUpdatedUtc { get; init; }
    public string? Message { get; init; }
}
