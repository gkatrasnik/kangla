import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { WateringCommandService } from '../../watering-command.service';
import { WateringCommand } from '../../watering-command';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-watering-commands-table',
  standalone: true,
  imports: [DatePipe, MatCardModule, MatTableModule, MatIconModule],
  templateUrl: './watering-commands-table.component.html',
  styleUrl: './watering-commands-table.component.scss'
})
export class WateringCommandsTableComponent implements OnInit, OnChanges {
  @Input({ required: true }) deviceId!: number;
  @Input() reloadTrigger = 0;
  commands: WateringCommand[] = [];
  displayedColumns = ['requestedAtUtc', 'status', 'durationSeconds', 'finishedAtUtc'];

  constructor(private wateringCommandService: WateringCommandService) {}

  getProgress(command: WateringCommand): 'queued' | 'watering' | 'completed' | 'failed' | 'timedOut' {
    if (command.status === 'pending') {
      return 'queued';
    }

    if (command.status === 'acknowledged') {
      return 'watering';
    }

    if (command.status === 'completed') {
      return 'completed';
    }

    return command.status === 'timedOut' ? 'timedOut' : 'failed';
  }

  getProgressLabel(command: WateringCommand): string {
    const progress = this.getProgress(command);
    if (progress === 'timedOut') {
      return 'Result not reported';
    }

    return progress.charAt(0).toUpperCase() + progress.slice(1);
  }

  getProgressIcon(command: WateringCommand): string {
    switch (this.getProgress(command)) {
      case 'queued': return 'schedule';
      case 'watering': return 'water_drop';
      case 'completed': return 'check_circle';
      case 'failed': return 'error';
      case 'timedOut': return 'help';
    }
  }

  getProgressDetail(command: WateringCommand): string | null {
    if (command.status === 'pending') {
      return 'Waiting for device';
    }

    if (command.status === 'acknowledged') {
      return 'Pump active';
    }

    if (command.status === 'cancelled') {
      return 'Command cancelled';
    }

    if (command.status === 'expired') {
      return 'Device did not respond';
    }

    if (command.status === 'timedOut') {
      return 'Device did not report completion';
    }

    return command.failureReason;
  }

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
