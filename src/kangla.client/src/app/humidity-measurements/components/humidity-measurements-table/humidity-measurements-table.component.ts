import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { HumidityMeasurementService } from '../../humidity-measurement.service';
import { HumidityMeasurement } from '../../humidity-measurement';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-humidity-measurements-table',
  standalone: true,
  imports: [DatePipe, MatCardModule, MatTableModule, MatPaginatorModule],
  templateUrl: './humidity-measurements-table.component.html',
  styleUrl: './humidity-measurements-table.component.scss'
})
export class HumidityMeasurementsTableComponent implements OnInit, OnChanges {
  @Input({ required: true }) deviceId!: number;
  @Input() reloadTrigger = 0;
  measurements: HumidityMeasurement[] = [];
  displayedColumns = ['dateTime', 'soilHumidity'];
  totalRecords = 0;
  pageSize = 10;
  pageIndex = 0;
  readonly pageSizeOptions = [10, 20, 50];

  constructor(private humidityMeasurementService: HumidityMeasurementService) {}

  ngOnInit(): void {
    this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['deviceId'] && !changes['deviceId'].firstChange) {
      this.pageIndex = 0;
      this.load();
    } else if (changes['reloadTrigger'] && !changes['reloadTrigger'].firstChange) {
      this.load();
    }
  }

  handlePageEvent(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.load();
  }

  private load(): void {
    if (!this.deviceId) {
      return;
    }

    this.humidityMeasurementService.getAll(this.deviceId, this.pageIndex + 1, this.pageSize).subscribe(response => {
      this.measurements = response.data;
      this.totalRecords = response.totalRecords;
    });
  }
}
