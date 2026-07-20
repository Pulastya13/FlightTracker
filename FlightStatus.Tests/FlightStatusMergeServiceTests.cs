using FlightStatus.Api.Models;
using FlightStatus.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlightStatus.Tests;

public class FlightStatusMergeServiceTests
{
    private readonly FlightStatusMergeService _service;
    private const string TestFlight = "SK1234";
    private static readonly DateOnly TestDate = new(2024, 3, 15);

    public FlightStatusMergeServiceTests()
    {
        _service = new FlightStatusMergeService(
            new TimeBasedStatusDerivationService(),
            NullLogger<FlightStatusMergeService>.Instance);
    }

    [Fact]
    public void Merge_BothProvidersRespond_PrefersLaterLastUpdatedUtc()
    {
        var olderResult = CreateResult("AeroTrack", new DateTime(2024, 3, 15, 7, 0, 0, DateTimeKind.Utc));
        var newerResult = CreateResult("QuickFlight", new DateTime(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc));

        var merged = _service.Merge([olderResult, newerResult], TestFlight, TestDate);

        Assert.Equal("QuickFlight", merged.ProviderSource);
    }

    [Fact]
    public void Merge_BothProvidersRespond_PrefersFirst_WhenFirstIsNewer()
    {
        var newerResult = CreateResult("AeroTrack", new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc));
        var olderResult = CreateResult("QuickFlight", new DateTime(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc));

        var merged = _service.Merge([newerResult, olderResult], TestFlight, TestDate);

        Assert.Equal("AeroTrack", merged.ProviderSource);
    }

    [Fact]
    public void Merge_OnlyOneProviderResponds_UsesThatResult()
    {
        var result = CreateResult("AeroTrack", new DateTime(2024, 3, 15, 7, 0, 0, DateTimeKind.Utc));

        var merged = _service.Merge([result, null], TestFlight, TestDate);

        Assert.Equal("AeroTrack", merged.ProviderSource);
    }

    [Fact]
    public void Merge_NeitherProviderResponds_ReturnsUnknown()
    {
        var merged = _service.Merge([null, null], TestFlight, TestDate);

        Assert.Equal(FlightStatusCode.Unknown, merged.Status);
        Assert.Equal("None", merged.ProviderSource);
        Assert.Equal("No flight data available from any provider", merged.Message);
    }

    [Fact]
    public void Merge_EmptyList_ReturnsUnknown()
    {
        var merged = _service.Merge([], TestFlight, TestDate);

        Assert.Equal(FlightStatusCode.Unknown, merged.Status);
        Assert.Equal("No flight data available from any provider", merged.Message);
    }

    [Fact]
    public void Merge_NullLastUpdatedUtc_TreatedAsOldest()
    {
        var withTimestamp = CreateResult("AeroTrack", new DateTime(2024, 3, 15, 7, 0, 0, DateTimeKind.Utc));
        var withoutTimestamp = new FlightStatusResult
        {
            FlightNumber = TestFlight,
            Date = TestDate,
            Status = FlightStatusCode.OnTime,
            ProviderSource = "QuickFlight",
            LastUpdatedUtc = null
        };

        var merged = _service.Merge([withTimestamp, withoutTimestamp], TestFlight, TestDate);

        Assert.Equal("AeroTrack", merged.ProviderSource);
    }

    private static FlightStatusResult CreateResult(string provider, DateTime lastUpdated)
    {
        return new FlightStatusResult
        {
            FlightNumber = TestFlight,
            Date = TestDate,
            Status = FlightStatusCode.OnTime,
            ProviderSource = provider,
            LastUpdatedUtc = lastUpdated
        };
    }
}
