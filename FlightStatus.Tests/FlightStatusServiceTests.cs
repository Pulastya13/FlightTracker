using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlightStatus.Tests;

public class FlightStatusServiceTests
{
    private readonly Mock<IFlightStatusProvider> _mockProvider1 = new();
    private readonly Mock<IFlightStatusProvider> _mockProvider2 = new();
    private readonly FlightStatusMergeService _mergeService;
    private const string TestFlight = "SK1234";
    private static readonly DateOnly TestDate = new(2024, 3, 15);

    public FlightStatusServiceTests()
    {
        _mergeService = new FlightStatusMergeService(
            new TimeBasedStatusDerivationService(),
            NullLogger<FlightStatusMergeService>.Instance);
    }

    private FlightStatusService CreateService()
    {
        return new FlightStatusService(
            [_mockProvider1.Object, _mockProvider2.Object],
            _mergeService,
            NullLogger<FlightStatusService>.Instance);
    }

    [Fact]
    public async Task GetFlightStatus_QueriesAllProviders()
    {
        _mockProvider1.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync((FlightStatusResult?)null);
        _mockProvider2.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync((FlightStatusResult?)null);

        var service = CreateService();
        await service.GetFlightStatusAsync(TestFlight, TestDate);

        _mockProvider1.Verify(p => p.GetFlightStatusAsync(TestFlight, TestDate, default), Times.Once);
        _mockProvider2.Verify(p => p.GetFlightStatusAsync(TestFlight, TestDate, default), Times.Once);
    }

    [Fact]
    public async Task GetFlightStatus_BothRespond_MergesCorrectly()
    {
        var older = new FlightStatusResult
        {
            FlightNumber = TestFlight,
            Date = TestDate,
            Status = FlightStatusCode.OnTime,
            ProviderSource = "Provider1",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 7, 0, 0, DateTimeKind.Utc)
        };
        var newer = new FlightStatusResult
        {
            FlightNumber = TestFlight,
            Date = TestDate,
            Status = FlightStatusCode.Delayed,
            ProviderSource = "Provider2",
            LastUpdatedUtc = new DateTime(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc)
        };

        _mockProvider1.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync(older);
        _mockProvider2.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync(newer);

        var service = CreateService();
        var result = await service.GetFlightStatusAsync(TestFlight, TestDate);

        Assert.Equal("Provider2", result.ProviderSource);
        Assert.Equal(FlightStatusCode.Delayed, result.Status);
    }

    [Fact]
    public async Task GetFlightStatus_NeitherResponds_ReturnsUnknown()
    {
        _mockProvider1.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync((FlightStatusResult?)null);
        _mockProvider2.Setup(p => p.GetFlightStatusAsync(TestFlight, TestDate, default))
            .ReturnsAsync((FlightStatusResult?)null);

        var service = CreateService();
        var result = await service.GetFlightStatusAsync(TestFlight, TestDate);

        Assert.Equal(FlightStatusCode.Unknown, result.Status);
        Assert.Contains("No flight data available", result.Message);
    }
}
