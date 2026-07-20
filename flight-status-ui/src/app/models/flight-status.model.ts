export enum FlightStatusCode {
  OnTime = 'OnTime',
  Delayed = 'Delayed',
  Cancelled = 'Cancelled',
  Diverted = 'Diverted',
  Unknown = 'Unknown',
}

export interface FlightStatusResult {
  flightNumber: string;
  date: string;
  status: FlightStatusCode;
  scheduledDeparture: string | null;
  actualDeparture: string | null;
  scheduledArrival: string | null;
  actualArrival: string | null;
  terminal: string | null;
  gate: string | null;
  delayReason: string | null;
  providerSource: string;
  lastUpdatedUtc: string | null;
  message: string | null;
}
