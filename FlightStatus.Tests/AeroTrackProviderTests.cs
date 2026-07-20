using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlightStatus.Tests;

public class AeroTrackProviderTests
{
    private readonly AeroTrackProvider _provider;

    public AeroTrackProviderTests()
    {
        _provider = new AeroTrackProvider(
            new AeroTrackStatusNormaliser(),
            NullLogger<AeroTrackProvider>.Instance);
    }

    [Fact]
    public async Task GetFlightStatus_KnownFlight_ReturnsResult()
    {
        var result = await _provider.GetFlightStatusAsync("SK1234", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal("SK1234", result.FlightNumber);
        Assert.Equal(FlightStatusCode.OnTime, result.Status);
        Assert.Equal("AeroTrack", result.ProviderSource);
    }

    [Fact]
    public async Task GetFlightStatus_KnownFlight_IncludesFullDetails()
    {
        var result = await _provider.GetFlightStatusAsync("BA456", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal("T5", result.Terminal);
        Assert.Equal("A22", result.Gate);
        Assert.Equal("Weather conditions at destination", result.DelayReason);
        Assert.Equal(FlightStatusCode.Delayed, result.Status);
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
    public async Task GetFlightStatus_CancelledFlight_ReturnsCancelledStatus()
    {
        var result = await _provider.GetFlightStatusAsync("LH789", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal(FlightStatusCode.Cancelled, result.Status);
    }

    [Fact]
    public async Task GetFlightStatus_DivertedFlight_ReturnsDivertedStatus()
    {
        var result = await _provider.GetFlightStatusAsync("AF101", new DateOnly(2024, 3, 15));

        Assert.NotNull(result);
        Assert.Equal(FlightStatusCode.Diverted, result.Status);
    }

    [Fact]
    public async Task GetFlightStatus_IsCaseInsensitive()
    {
        var result = await _provider.GetFlightStatusAsync("sk1234", new DateOnly(2024, 3, 15));
        Assert.NotNull(result);
    }

    [Fact]
    public void ProviderName_ReturnsAeroTrack()
    {
        Assert.Equal("AeroTrack", _provider.ProviderName);
    }
}
