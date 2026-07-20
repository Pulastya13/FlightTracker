# Flight Status Tracker

A Flight Status lookup feature for the **SkyRoute** platform. A support agent enters a flight number and date; the system queries two stub providers (AeroTrack and QuickFlight), normalises their responses into a unified status model, and displays the result.

---

## Tech Stack

| Layer    | Technology            |
|----------|-----------------------|
| Backend  | .NET 9 Minimal API (C#) |
| Frontend | Angular 19            |
| Tests    | xUnit + Moq           |

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (tested with 9.0.305)
- [Node.js 18+](https://nodejs.org/) (tested with v22)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)

---

## Setup & Run

### 1. Clone the repository

```bash
git clone <repo-url>
cd FlightTracker
```

### 2. Run the Backend API

```bash
cd FlightStatus.Api
dotnet run --launch-profile http
```

The API will start at `http://localhost:5000`.

### 3. Run the Frontend

```bash
cd flight-status-ui
npm install
npx ng serve
```

The UI will be available at `http://localhost:4200`.

### 4. Run Tests

```bash
dotnet test
```

---

## API Endpoint

```
GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
```

### Test Flight Numbers

| Flight   | Date       | Expected Status | Notes                          |
|----------|------------|-----------------|--------------------------------|
| SK1234   | 2024-03-15 | OnTime          | Both providers, AeroTrack newer |
| BA456    | 2024-03-15 | Delayed         | Both providers, QuickFlight newer |
| LH789    | 2024-03-15 | Cancelled       | Both providers, AeroTrack newer |
| AF101    | 2024-03-15 | Diverted        | AeroTrack only                 |
| UA303    | 2024-03-15 | Diverted        | QuickFlight only               |
| EK502    | 2024-03-16 | OnTime          | Both providers, QuickFlight newer |
| XX9999   | any date   | Unknown         | Not in any provider            |

---

## Architecture

```
Angular UI (4200) ──▶ .NET Minimal API (5000)
                         │
                         ├── AeroTrackProvider (stub)
                         └── QuickFlightProvider (stub)
                         │
                         ├── StatusNormalisationService
                         └── FlightStatusMergeService
```

### Design Patterns

- **Strategy Pattern**: `IFlightStatusProvider` allows adding/swapping providers
- **Dependency Injection**: All services registered in DI container
- **Single Responsibility**: Separate normalisation, merging, and provider concerns
- **Open/Closed Principle**: New providers without modifying existing code
- **Interface Segregation**: Minimal interfaces for each concern

---

## Assumptions

1. Stubs use deterministic, hardcoded data — no randomness or real API calls
2. Flight number matching is case-insensitive
3. Date matching compares against the scheduled departure date
4. The "later lastUpdatedUtc" merge rule compares UTC timestamps directly
5. No authentication, persistence, or real-time updates required
6. Application runs fully offline on localhost
