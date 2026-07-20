using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Orchestrates querying all registered providers and merging results.
/// Acts as the main entry point for flight status lookups.
/// </summary>
public interface IFlightStatusService
{
    Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default);
}
