using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Normalises QuickFlight-specific status vocabulary to unified FlightStatusCode.
/// Owns the mapping for QuickFlight provider only (Single Responsibility).
/// </summary>
public class QuickFlightStatusNormaliser : IStatusNormaliser
{
    private static readonly Dictionary<string, FlightStatusCode> Mapping = new(StringComparer.OrdinalIgnoreCase)
    {
        ["punctual"] = FlightStatusCode.OnTime,
        ["behind_schedule"] = FlightStatusCode.Delayed,
        ["cancelled"] = FlightStatusCode.Cancelled,
        ["diverted"] = FlightStatusCode.Diverted
    };

    public FlightStatusCode Normalise(string providerStatus)
    {
        return Mapping.TryGetValue(providerStatus, out var status)
            ? status
            : FlightStatusCode.Unknown;
    }
}
