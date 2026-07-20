import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FlightStatusResult,
  FlightStatusCode,
} from '../../models/flight-status.model';

@Component({
  selector: 'app-flight-result',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './flight-result.component.html',
  styleUrl: './flight-result.component.scss',
})
export class FlightResultComponent {
  @Input() result!: FlightStatusResult;

  get statusClass(): string {
    switch (this.result.status) {
      case FlightStatusCode.OnTime:
        return 'status-ontime';
      case FlightStatusCode.Delayed:
        return 'status-delayed';
      case FlightStatusCode.Cancelled:
      case FlightStatusCode.Diverted:
        return 'status-critical';
      default:
        return 'status-unknown';
    }
  }

  get statusLabel(): string {
    switch (this.result.status) {
      case FlightStatusCode.OnTime:
        return 'On Time';
      case FlightStatusCode.Delayed:
        return 'Delayed';
      case FlightStatusCode.Cancelled:
        return 'Cancelled';
      case FlightStatusCode.Diverted:
        return 'Diverted';
      default:
        return 'Unknown';
    }
  }

  get hasAeroTrackDetails(): boolean {
    return !!(this.result.terminal || this.result.gate || this.result.delayReason);
  }

  formatDateTime(dateStr: string | null): string {
    if (!dateStr) return '—';
    const date = new Date(dateStr);
    return date.toLocaleString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      timeZoneName: 'short',
    });
  }
}
