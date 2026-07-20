using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;

namespace FlightStatus.Api.Services;

public class FlightStatusService : IFlightStatusService
{
    private readonly IEnumerable<IFlightStatusProvider> _providers;
    private readonly IFlightStatusMergeService _mergeService;
    private readonly ILogger<FlightStatusService> _logger;

    public FlightStatusService(
        IEnumerable<IFlightStatusProvider> providers,
        IFlightStatusMergeService mergeService,
        ILogger<FlightStatusService> logger)
    {
        _providers = providers;
        _mergeService = mergeService;
        _logger = logger;
    }

    public async Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Looking up status for flight {FlightNumber} on {Date}", flightNumber, date);

        var tasks = _providers.Select(p => SafeQueryProvider(p, flightNumber, date, cancellationToken));
        var results = await Task.WhenAll(tasks);

        var nonNullResults = results.Count(r => r != null);
        _logger.LogInformation("Received {Count} provider responses for {FlightNumber}", nonNullResults, flightNumber);

        return _mergeService.Merge(results, flightNumber, date);
    }

    private async Task<FlightStatusResult?> SafeQueryProvider(
        IFlightStatusProvider provider, string flightNumber, DateOnly date, CancellationToken ct)
    {
        try
        {
            return await provider.GetFlightStatusAsync(flightNumber, date, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider {Provider} failed for flight {FlightNumber}", provider.ProviderName, flightNumber);
            return null;
        }
    }
}
