using FlightStatus.Api.Models;
using FlightStatus.Api.Services;

namespace FlightStatus.Tests;

public class AeroTrackStatusNormaliserTests
{
    private readonly AeroTrackStatusNormaliser _normaliser = new();

    [Theory]
    [InlineData("ON_SCHEDULE", FlightStatusCode.OnTime)]
    [InlineData("LATE", FlightStatusCode.Delayed)]
    [InlineData("NO_FLY", FlightStatusCode.Cancelled)]
    [InlineData("REROUTED", FlightStatusCode.Diverted)]
    public void Normalise_KnownStatuses_MapsCorrectly(string input, FlightStatusCode expected)
    {
        var result = _normaliser.Normalise(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("UNKNOWN_STATUS")]
    [InlineData("")]
    [InlineData("random")]
    public void Normalise_UnknownStatuses_ReturnsUnknown(string input)
    {
        var result = _normaliser.Normalise(input);
        Assert.Equal(FlightStatusCode.Unknown, result);
    }

    [Fact]
    public void Normalise_IsCaseInsensitive()
    {
        Assert.Equal(FlightStatusCode.OnTime, _normaliser.Normalise("on_schedule"));
        Assert.Equal(FlightStatusCode.Delayed, _normaliser.Normalise("late"));
    }
}

public class QuickFlightStatusNormaliserTests
{
    private readonly QuickFlightStatusNormaliser _normaliser = new();

    [Theory]
    [InlineData("punctual", FlightStatusCode.OnTime)]
    [InlineData("behind_schedule", FlightStatusCode.Delayed)]
    [InlineData("cancelled", FlightStatusCode.Cancelled)]
    [InlineData("diverted", FlightStatusCode.Diverted)]
    public void Normalise_KnownStatuses_MapsCorrectly(string input, FlightStatusCode expected)
    {
        var result = _normaliser.Normalise(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("UNKNOWN")]
    [InlineData("")]
    [InlineData("something_else")]
    public void Normalise_UnknownStatuses_ReturnsUnknown(string input)
    {
        var result = _normaliser.Normalise(input);
        Assert.Equal(FlightStatusCode.Unknown, result);
    }

    [Fact]
    public void Normalise_IsCaseInsensitive()
    {
        Assert.Equal(FlightStatusCode.OnTime, _normaliser.Normalise("PUNCTUAL"));
        Assert.Equal(FlightStatusCode.Delayed, _normaliser.Normalise("BEHIND_SCHEDULE"));
    }
}
