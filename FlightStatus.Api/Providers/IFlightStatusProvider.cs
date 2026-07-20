using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Abstraction for flight status data providers.
/// Implementations normalise their native response into the unified FlightStatusResult.
/// </summary>
public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<FlightStatusResult?> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default);
}
