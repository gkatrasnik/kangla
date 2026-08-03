import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { NotificationService } from '../core/notifications/notification.service';
import { WateringEvent } from '../watering-events/watering-event';
import { WateringEventService } from '../watering-events/watering-event.service';
import { Plant } from './plant';

@Injectable({ providedIn: 'root' })
export class PlantWateringActionService {
  constructor(
    private wateringEventService: WateringEventService,
    private notificationService: NotificationService
  ) {}

  markAsWatered(plant: Plant, onChanged?: () => void): Observable<WateringEvent> {
    const previousWateringDate = plant.lastWateringDateTime;
    const start = new Date();

    return this.wateringEventService.addWateringEvent({
      plantId: plant.id,
      start,
      end: new Date(start.getTime() + 10000)
    }).pipe(
      tap(event => {
        plant.lastWateringDateTime = start;
        onChanged?.();

        this.notificationService.showUndoSnackBar(`${plant.name} marked as watered`).subscribe(() => {
          this.wateringEventService.deleteWateringEvent(event.id).subscribe({
            next: () => {
              plant.lastWateringDateTime = previousWateringDate;
              onChanged?.();
              this.notificationService.showNonErrorSnackBar('Watering entry removed');
            },
            error: () => this.notificationService.showClientError('Could not undo the watering entry')
          });
        });
      })
    );
  }
}
