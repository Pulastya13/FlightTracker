import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlightStatusService } from '../../services/flight-status.service';
import { FlightStatusResult } from '../../models/flight-status.model';
import { FlightResultComponent } from '../flight-result/flight-result.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-flight-search',
  standalone: true,
  imports: [FormsModule, CommonModule, FlightResultComponent],
  templateUrl: './flight-search.component.html',
  styleUrl: './flight-search.component.scss',
})
export class FlightSearchComponent {
  flightNumber = '';
  date = '';
  result: FlightStatusResult | null = null;
  error: string | null = null;
  loading = false;

  constructor(private flightStatusService: FlightStatusService) {}

  search(): void {
    if (!this.flightNumber.trim() || !this.date) {
      this.error = 'Please enter both flight number and date.';
      return;
    }

    this.loading = true;
    this.error = null;
    this.result = null;

    this.flightStatusService
      .getFlightStatus(this.flightNumber.trim(), this.date)
      .subscribe({
        next: (data) => {
          this.result = data;
          this.loading = false;
        },
        error: (err) => {
          this.error =
            err.error?.error ||
            'Failed to fetch flight status. Please try again.';
          this.loading = false;
        },
      });
  }
}
