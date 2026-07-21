using FlightStatus.Api.Helpers;

namespace FlightStatus.Tests;

public class FlightNumberValidatorTests
{
    [Theory]
    [InlineData("SK1234")]
    [InlineData("BA456")]
    [InlineData("LH7")]
    [InlineData("AF1")]
    [InlineData("EK5020")]
    public void IsValid_ValidFlightNumbers_ReturnsTrue(string flightNumber)
    {
        Assert.True(FlightNumberValidator.IsValid(flightNumber));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("S1234")]
    [InlineData("SKY1234")]
    [InlineData("SK")]
    [InlineData("SK12345")]
    [InlineData("sk1234")]  // lowercase is valid (normalised internally)
    public void IsValid_InvalidFlightNumbers_ReturnsFalse(string flightNumber)
    {
        // Note: "sk1234" should be valid since we uppercase internally
        if (flightNumber == "sk1234")
            Assert.True(FlightNumberValidator.IsValid(flightNumber));
        else
            Assert.False(FlightNumberValidator.IsValid(flightNumber));
    }

    [Fact]
    public void IsValid_Null_ReturnsFalse()
    {
        Assert.False(FlightNumberValidator.IsValid(null));
    }

    [Fact]
    public void IsValid_WhitespaceAroundValid_ReturnsTrue()
    {
        Assert.True(FlightNumberValidator.IsValid(" SK1234 "));
    }
}
