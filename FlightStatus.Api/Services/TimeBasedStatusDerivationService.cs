using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Derives flight status from time data when the provider status is Unknown or as a validation fallback.
/// Rule: if |ActualDeparture - ScheduledDeparture| <= 15 minutes
///       OR |ActualArrival - ScheduledArrival| <= 15 minutes → OnTime, else → Delayed.
/// Returns Unknown when no actual time data is available.
/// </summary>
public interface ITimeBasedStatusDerivationService
{
    FlightStatusCode DeriveStatus(
        DateTime? scheduledDeparture, DateTime? actualDeparture,
        DateTime? scheduledArrival, DateTime? actualArrival);
}

public class TimeBasedStatusDerivationService : ITimeBasedStatusDerivationService
{
    private static readonly TimeSpan OnTimeThreshold = TimeSpan.FromMinutes(15);

    public FlightStatusCode DeriveStatus(
        DateTime? scheduledDeparture, DateTime? actualDeparture,
        DateTime? scheduledArrival, DateTime? actualArrival)
    {
        var departureOnTime = IsWithinThreshold(scheduledDeparture, actualDeparture);
        var arrivalOnTime = IsWithinThreshold(scheduledArrival, actualArrival);

        // No comparable time pair available
        if (departureOnTime is null && arrivalOnTime is null)
            return FlightStatusCode.Unknown;

        // OR rule: on time if either leg is within threshold
        return departureOnTime == true || arrivalOnTime == true
            ? FlightStatusCode.OnTime
            : FlightStatusCode.Delayed;
    }

    private static bool? IsWithinThreshold(DateTime? scheduled, DateTime? actual)
    {
        if (scheduled is null || actual is null)
            return null;

        return (actual.Value - scheduled.Value).Duration() <= OnTimeThreshold;
    }
}
