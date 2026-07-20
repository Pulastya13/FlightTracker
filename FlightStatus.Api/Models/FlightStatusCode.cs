namespace FlightStatus.Api.Models;

/// <summary>
/// Unified flight status enum normalised from all provider vocabularies.
/// </summary>
public enum FlightStatusCode
{
    OnTime,
    Delayed,
    Cancelled,
    Diverted,
    Unknown
}
