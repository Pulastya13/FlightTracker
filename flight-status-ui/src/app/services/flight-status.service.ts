import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FlightStatusResult } from '../models/flight-status.model';

@Injectable({
  providedIn: 'root',
})
export class FlightStatusService {
  private readonly apiUrl = 'http://localhost:5000/flights/status';

  constructor(private http: HttpClient) {}

  getFlightStatus(
    flightNumber: string,
    date: string
  ): Observable<FlightStatusResult> {
    const params = new HttpParams()
      .set('flightNumber', flightNumber)
      .set('date', date);

    return this.http.get<FlightStatusResult>(this.apiUrl, { params });
  }
}
