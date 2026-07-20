using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Normalises AeroTrack-specific status vocabulary to unified FlightStatusCode.
/// Owns the mapping for AeroTrack provider only (Single Responsibility).
/// </summary>
public class AeroTrackStatusNormaliser : IStatusNormaliser
{
    private static readonly Dictionary<string, FlightStatusCode> Mapping = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ON_SCHEDULE"] = FlightStatusCode.OnTime,
        ["LATE"] = FlightStatusCode.Delayed,
        ["NO_FLY"] = FlightStatusCode.Cancelled,
        ["REROUTED"] = FlightStatusCode.Diverted
    };

    public FlightStatusCode Normalise(string providerStatus)
    {
        return Mapping.TryGetValue(providerStatus, out var status)
            ? status
            : FlightStatusCode.Unknown;
    }
}
