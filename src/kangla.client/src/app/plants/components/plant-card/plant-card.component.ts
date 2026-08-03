import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { Plant } from '../../plant';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';
import { NotificationService } from '../../../core/notifications/notification.service';
import { PlantService } from '../../plant.service';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';

@Component({
  selector: 'app-plant-card',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, MatIconModule, RouterLink, ImageSrcDirective],
  templateUrl: './plant-card.component.html',
  styleUrl: './plant-card.component.scss'
})
export class PlantCardComponent {
  @Input() plant!: Plant;
  @Input() imageUrl!: string | undefined;
  @Input() wateringDevice: WateringDevice | null = null;
  watering = false;
  sendingDeviceCommand = false;
/**
 * Initializes a new instance of the PlantCardComponent class.
 * @param wateringEventService - Service to handle watering events.
 * @param notificationService - Service to display notifications.
 * @param plantService - Service to manage plant data.
 */
  constructor ( 
    private wateringActionService: PlantWateringActionService,
    private deviceWateringActionService: DeviceWateringActionService,
    private notificationService: NotificationService,
    public plantService:  PlantService
  ) {}

  triggerWatering() {
    if (this.watering) {
      return;
    }

    this.watering = true;
    this.wateringActionService.markAsWatered(this.plant).pipe(
      finalize(() => this.watering = false)
    ).subscribe({
      next: () => {},
      error: () => this.notificationService.showClientError(`Could not update ${this.plant.name}`)
    });
  }

  sendWateringCommand(): void {
    if (!this.wateringDevice || this.sendingDeviceCommand) {
      return;
    }

    this.sendingDeviceCommand = true;
    this.deviceWateringActionService.send(this.wateringDevice, this.plant.name).pipe(
      finalize(() => this.sendingDeviceCommand = false)
    ).subscribe();
  }
}
