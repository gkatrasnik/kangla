import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { HumidityMeasurementService } from '../../humidity-measurement.service';
import { HumidityMeasurement } from '../../humidity-measurement';

@Component({
  selector: 'app-humidity-measurements-table',
  standalone: true,
  imports: [DatePipe, MatCardModule, MatTableModule],
  templateUrl: './humidity-measurements-table.component.html',
  styleUrl: './humidity-measurements-table.component.scss'
})
export class HumidityMeasurementsTableComponent implements OnInit, OnChanges {
  @Input({ required: true }) deviceId!: number;
  @Input() reloadTrigger = 0;
  measurements: HumidityMeasurement[] = [];
  displayedColumns = ['dateTime', 'soilMoisturePercentage', 'rawSoilMoisture'];

  constructor(private humidityMeasurementService: HumidityMeasurementService) {}

  ngOnInit(): void {
    this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['reloadTrigger'] && !changes['reloadTrigger'].firstChange) {
      this.load();
    }
  }

  private load(): void {
    if (!this.deviceId) {
      return;
    }

    this.humidityMeasurementService.getAll(this.deviceId, 1, 10).subscribe(response => this.measurements = response.data);
  }
}
