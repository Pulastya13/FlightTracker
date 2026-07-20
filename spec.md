# Flight Status Tracker — Specification

## Overview

A Flight Status lookup feature for the SkyRoute platform. A support agent enters a flight number and date; the system queries two stub providers, normalises their responses into a unified status model, and displays the result.

---

## Data Models

### Unified Flight Status Enum

```csharp
public enum FlightStatus
{
    OnTime,     // Departure or arrival within 15 minutes of schedule
    Delayed,    // Departure or arrival pushed beyond 15 minutes
    Cancelled,  // Flight will not operate
    Diverted,   // Flight landed at a different airport
    Unknown     // No usable status returned by either provider
}
```

### FlightStatusResult (API Response DTO)

```csharp
public record FlightStatusResult
{
    public string FlightNumber { get; init; }
    public DateOnly Date { get; init; }
    public FlightStatus Status { get; init; }
    public DateTime? ScheduledDeparture { get; init; }
    public DateTime? ActualDeparture { get; init; }
    public DateTime? ScheduledArrival { get; init; }
    public DateTime? ActualArrival { get; init; }
    public string? Terminal { get; init; }          // AeroTrack only
    public string? Gate { get; init; }              // AeroTrack only
    public string? DelayReason { get; init; }       // AeroTrack only
    public string ProviderSource { get; init; }     // Which provider data came from
    public DateTime? LastUpdatedUtc { get; init; }
    public string? Message { get; init; }           // For error/info messages
}
```

### Provider Response Models

#### AeroTrackResponse

```csharp
public record AeroTrackResponse
{
    public string FlightNumber { get; init; }
    public string Status { get; init; }             // e.g., "ON_SCHEDULE", "LATE", "NO_FLY", "REROUTED"
    public DateTime ScheduledDeparture { get; init; }
    public DateTime? ActualDeparture { get; init; }
    public DateTime ScheduledArrival { get; init; }
    public DateTime? ActualArrival { get; init; }
    public string? Terminal { get; init; }
    public string? Gate { get; init; }
    public string? DelayReason { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}
```

#### QuickFlightResponse

```csharp
public record QuickFlightResponse
{
    public string FlightCode { get; init; }
    public string CurrentStatus { get; init; }      // e.g., "punctual", "behind_schedule", "cancelled", "diverted"
    public DateTime DepartureScheduled { get; init; }
    public DateTime ArrivalScheduled { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}
```

---

## Status Vocabulary Mapping

| Unified Status | AeroTrack Vocabulary | QuickFlight Vocabulary |
|---------------|---------------------|----------------------|
| OnTime        | `ON_SCHEDULE`       | `punctual`           |
| Delayed       | `LATE`              | `behind_schedule`    |
| Cancelled     | `NO_FLY`            | `cancelled`          |
| Diverted      | `REROUTED`          | `diverted`           |
| Unknown       | *(unmapped value)*  | *(unmapped value)*   |

---

## Interface Contracts

### IFlightStatusProvider

```csharp
public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<FlightStatusResult?> GetFlightStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default);
}
```

Each provider implementation:
1. Looks up the flight in its deterministic stub data
2. Maps its native response to the unified `FlightStatusResult`
3. Returns `null` if the flight is not found

---

## Merge Rules

1. **Both providers respond** → Use the result with the later `LastUpdatedUtc`
2. **One provider responds** → Use that result
3. **Neither responds** → Return `FlightStatusResult` with `Status = Unknown` and `Message = "No flight data available from any provider"`

---

## Normalisation Rules

### Time-Based Status Derivation

When a provider returns raw times without an explicit cancelled/diverted status:
- If `|ActualDeparture - ScheduledDeparture| <= 15 minutes` OR `|ActualArrival - ScheduledArrival| <= 15 minutes` → **OnTime**
- If delay exceeds 15 minutes → **Delayed**

### Priority
The provider's explicit status string takes precedence. Time-based derivation is a fallback validation.

---

## API Contract

### Endpoint

```
GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
```

### Success Response (200 OK)

```json
{
    "flightNumber": "SK1234",
    "date": "2024-03-15",
    "status": "Delayed",
    "scheduledDeparture": "2024-03-15T08:00:00Z",
    "actualDeparture": "2024-03-15T08:45:00Z",
    "scheduledArrival": "2024-03-15T11:00:00Z",
    "actualArrival": null,
    "terminal": "T2",
    "gate": "B14",
    "delayReason": "Weather conditions",
    "providerSource": "AeroTrack",
    "lastUpdatedUtc": "2024-03-15T07:30:00Z",
    "message": null
}
```

### Validation Error (400 Bad Request)

```json
{
    "error": "flightNumber and date are required query parameters"
}
```

### Not Found (when neither provider has data)

```json
{
    "flightNumber": "XX9999",
    "date": "2024-03-15",
    "status": "Unknown",
    "scheduledDeparture": null,
    "actualDeparture": null,
    "scheduledArrival": null,
    "actualArrival": null,
    "terminal": null,
    "gate": null,
    "delayReason": null,
    "providerSource": "None",
    "lastUpdatedUtc": null,
    "message": "No flight data available from any provider"
}
```

---

## Architecture

```
┌─────────────┐     ┌──────────────────────┐     ┌────────────────────────┐
│  Angular UI │────▶│  .NET Minimal API     │────▶│  IFlightStatusProvider │
│             │◀────│  /flights/status      │◀────│  ├─ AeroTrackProvider  │
└─────────────┘     │                      │     │  └─ QuickFlightProvider│
                    │  FlightStatusService  │     └────────────────────────┘
                    │  (Normalise + Merge)  │
                    └──────────────────────┘
```

### Design Patterns

- **Strategy Pattern**: `IFlightStatusProvider` allows swapping/adding providers without modifying consumers
- **Dependency Injection**: All providers registered in DI container
- **Single Responsibility**: Separate normalisation, merging, and provider concerns
- **Open/Closed Principle**: New providers can be added without modifying existing code
- **Interface Segregation**: Providers implement only what they need

---

## Test Scenarios

| Scenario | Expected Result |
|----------|----------------|
| Flight found in both providers, AeroTrack newer | Use AeroTrack data |
| Flight found in both providers, QuickFlight newer | Use QuickFlight data |
| Flight only in AeroTrack | Use AeroTrack data |
| Flight only in QuickFlight | Use QuickFlight data |
| Flight not in either provider | Status = Unknown with message |
| Missing flightNumber param | 400 Bad Request |
| Missing date param | 400 Bad Request |
| AeroTrack status ON_SCHEDULE | Maps to OnTime |
| AeroTrack status LATE | Maps to Delayed |
| AeroTrack status NO_FLY | Maps to Cancelled |
| AeroTrack status REROUTED | Maps to Diverted |
| QuickFlight status punctual | Maps to OnTime |
| QuickFlight status behind_schedule | Maps to Delayed |
| QuickFlight status cancelled | Maps to Cancelled |
| QuickFlight status diverted | Maps to Diverted |
