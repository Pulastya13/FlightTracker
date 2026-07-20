using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlightStatus.Tests;

public class QuickFlightProviderTests
{
    private readonly QuickFlightProvider _provider;

    public QuickFlightProviderTests()
    {
        _provider = new QuickFlightProvider(
            new QuickFlightStatusNormaliser(),
            NullLogger<QuickFlightProvider>.Instance);
    }

    [Fact]
    public async Task GetFlightStatus_KnownFlight_ReturnsResult()
    {
        var result = await _provider.GetFlightStatusAsync("SK1234", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal("SK1234", result.FlightNumber);
        Assert.Equal(FlightStatusCode.OnTime, result.Status);
        Assert.Equal("QuickFlight", result.ProviderSource);
    }

    [Fact]
    public async Task GetFlightStatus_MinimalDetails_NoExtraFields()
    {
        var result = await _provider.GetFlightStatusAsync("BA456", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Null(result.Terminal);
        Assert.Null(result.Gate);
        Assert.Null(result.DelayReason);
        Assert.Null(result.ActualDeparture);
        Assert.Null(result.ActualArrival);
    }

    [Fact]
    public async Task GetFlightStatus_UnknownFlight_ReturnsNull()
    {
        var result = await _provider.GetFlightStatusAsync("XX9999", new DateOnly(2024, 3, 15));
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFlightStatus_WrongDate_ReturnsNull()
    {
        var result = await _provider.GetFlightStatusAsync("SK1234", new DateOnly(2024, 3, 16));
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFlightStatus_DivertedFlight_ReturnsDivertedStatus()
    {
        var result = await _provider.GetFlightStatusAsync("UA303", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal(FlightStatusCode.Diverted, result.Status);
    }

    [Fact]
    public async Task GetFlightStatus_CancelledFlight_ReturnsCancelledStatus()
    {
        var result = await _provider.GetFlightStatusAsync("LH789", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal(FlightStatusCode.Cancelled, result.Status);
    }

    [Fact]
    public void ProviderName_ReturnsQuickFlight()
    {
        Assert.Equal("QuickFlight", _provider.ProviderName);
    }
}
