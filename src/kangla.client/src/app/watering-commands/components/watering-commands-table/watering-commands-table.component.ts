import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { WateringCommandService } from '../../watering-command.service';
import { WateringCommand } from '../../watering-command';

@Component({
  selector: 'app-watering-commands-table',
  standalone: true,
  imports: [DatePipe, MatCardModule, MatTableModule],
  templateUrl: './watering-commands-table.component.html'
})
export class WateringCommandsTableComponent implements OnInit, OnChanges {
  @Input({ required: true }) deviceId!: number;
  @Input() reloadTrigger = 0;
  commands: WateringCommand[] = [];
  displayedColumns = ['requestedAtUtc', 'status', 'durationSeconds', 'finishedAtUtc'];

  constructor(private wateringCommandService: WateringCommandService) {}

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

    this.wateringCommandService.getAll(this.deviceId, 1, 10).subscribe(response => this.commands = response.data);
  }
}
