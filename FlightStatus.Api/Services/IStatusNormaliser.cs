using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Strategy interface for normalising provider-specific status strings
/// into the unified FlightStatusCode enum.
/// Each provider implements its own normalisation strategy.
/// </summary>
public interface IStatusNormaliser
{
    FlightStatusCode Normalise(string providerStatus);
}
