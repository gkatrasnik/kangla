import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { Plant } from '../../plant';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';
import { WateringEventCreateRequestDto } from '../../../watering-events/dto/watering-event-create-request-dto';
import { WateringEventService } from '../../../watering-events/watering-event.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { PlantService } from '../../plant.service';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';

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
  @Input() hasWateringDevice = false;
  watering = false;
/**
 * Initializes a new instance of the PlantCardComponent class.
 * @param wateringEventService - Service to handle watering events.
 * @param notificationService - Service to display notifications.
 * @param plantService - Service to manage plant data.
 */
  constructor ( 
    private wateringEventService: WateringEventService,
    private notificationService: NotificationService,
    public plantService:  PlantService
  ) {}

  triggerWatering() {
    if (this.watering) {
      return;
    }

    const start = new Date();
    const end = new Date(start.getTime() + 10000); // End time 10 seconds after start

    const wateringEvent: WateringEventCreateRequestDto = {
      plantId: this.plant.id,
      start: start,
      end: end
    };

    this.watering = true;
    this.wateringEventService.addWateringEvent(wateringEvent).pipe(
      finalize(() => this.watering = false)
    ).subscribe({
      next: () => {
        this.notificationService.showNonErrorSnackBar(`${this.plant.name} marked as watered`);
        this.plant.lastWateringDateTime = new Date();
      },
      error: () => this.notificationService.showClientError(`Could not update ${this.plant.name}`)
    });
  }
}
