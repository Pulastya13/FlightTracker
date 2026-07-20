using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// Merges results from multiple providers according to business rules.
/// </summary>
public interface IFlightStatusMergeService
{
    FlightStatusResult Merge(IReadOnlyList<FlightStatusResult?> results, string flightNumber, DateOnly date);
}
