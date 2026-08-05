import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import {
  ActiveWateringCommandStatus,
  getActiveWateringCommandLabel
} from '../../watering-command';

@Component({
  selector: 'app-watering-command-status-badge',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './watering-command-status-badge.component.html',
  styleUrl: './watering-command-status-badge.component.scss'
})
export class WateringCommandStatusBadgeComponent {
  @Input({ required: true }) status!: ActiveWateringCommandStatus;

  get label(): string {
    return getActiveWateringCommandLabel(this.status);
  }

  get icon(): string {
    return this.status === 'pending' ? 'schedule' : 'water_drop';
  }
}
