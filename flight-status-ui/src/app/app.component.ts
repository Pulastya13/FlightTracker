import { Component } from '@angular/core';
import { FlightSearchComponent } from './components/flight-search/flight-search.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FlightSearchComponent],
  template: '<app-flight-search></app-flight-search>',
  styleUrl: './app.component.scss',
})
export class AppComponent {}
