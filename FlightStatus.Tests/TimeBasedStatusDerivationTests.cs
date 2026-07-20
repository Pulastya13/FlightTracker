using FlightStatus.Api.Models;
using FlightStatus.Api.Services;

namespace FlightStatus.Tests;

public class TimeBasedStatusDerivationTests
{
    private readonly TimeBasedStatusDerivationService _service = new();

    private static readonly DateTime SchedDep = new(2024, 3, 15, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SchedArr = new(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void DeriveStatus_DepartureWithin15Minutes_ReturnsOnTime()
    {
        var result = _service.DeriveStatus(SchedDep, SchedDep.AddMinutes(10), null, null);
        Assert.Equal(FlightStatusCode.OnTime, result);
    }

    [Fact]
    public void DeriveStatus_DepartureExactly15Minutes_ReturnsOnTime()
    {
        var result = _service.DeriveStatus(SchedDep, SchedDep.AddMinutes(15), null, null);
        Assert.Equal(FlightStatusCode.OnTime, result);
    }

    [Fact]
    public void DeriveStatus_DepartureOver15Minutes_ReturnsDelayed()
    {
        var result = _service.DeriveStatus(SchedDep, SchedDep.AddMinutes(45), null, null);
        Assert.Equal(FlightStatusCode.Delayed, result);
    }

    [Fact]
    public void DeriveStatus_NoActualTimes_ReturnsUnknown()
    {
        var result = _service.DeriveStatus(SchedDep, null, SchedArr, null);
        Assert.Equal(FlightStatusCode.Unknown, result);
    }

    [Fact]
    public void DeriveStatus_NoTimesAtAll_ReturnsUnknown()
    {
        var result = _service.DeriveStatus(null, null, null, null);
        Assert.Equal(FlightStatusCode.Unknown, result);
    }

    [Fact]
    public void DeriveStatus_EarlyDeparture_ReturnsOnTime()
    {
        var result = _service.DeriveStatus(SchedDep, SchedDep.AddMinutes(-10), null, null);
        Assert.Equal(FlightStatusCode.OnTime, result);
    }

    [Fact]
    public void DeriveStatus_ArrivalOnlyWithin15Minutes_ReturnsOnTime()
    {
        // No departure data — derivable from arrival alone (spec OR rule)
        var result = _service.DeriveStatus(null, null, SchedArr, SchedArr.AddMinutes(5));
        Assert.Equal(FlightStatusCode.OnTime, result);
    }

    [Fact]
    public void DeriveStatus_ArrivalOnlyOver15Minutes_ReturnsDelayed()
    {
        var result = _service.DeriveStatus(null, null, SchedArr, SchedArr.AddMinutes(30));
        Assert.Equal(FlightStatusCode.Delayed, result);
    }

    [Fact]
    public void DeriveStatus_LateDepartureButOnTimeArrival_ReturnsOnTime()
    {
        // OR rule: departure delayed 40min but arrival within 15min => OnTime
        var result = _service.DeriveStatus(
            SchedDep, SchedDep.AddMinutes(40),
            SchedArr, SchedArr.AddMinutes(10));
        Assert.Equal(FlightStatusCode.OnTime, result);
    }

    [Fact]
    public void DeriveStatus_BothLegsLate_ReturnsDelayed()
    {
        var result = _service.DeriveStatus(
            SchedDep, SchedDep.AddMinutes(40),
            SchedArr, SchedArr.AddMinutes(35));
        Assert.Equal(FlightStatusCode.Delayed, result);
    }
}
