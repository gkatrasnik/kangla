import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Plant } from '../../plant';
import { PlantService } from '../../plant.service';
import { ImagesService } from '../../../shared/services/images.service';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringEventService } from '../../../watering-events/watering-event.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule, ImageSrcDirective],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.scss'
})
export class OverviewComponent implements OnInit {
  plants: Plant[] = [];
  devicePlantIds = new Set<number>();
  wateringPlantIds = new Set<number>();
  loading = true;
  loadError = false;

  constructor(
    public plantService: PlantService,
    public imagesService: ImagesService,
    private wateringDeviceService: WateringDeviceService,
    private wateringEventService: WateringEventService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadOverview();
  }

  get overduePlants(): Plant[] {
    return this.plants
      .filter(plant => this.plantService.isWateringOverdue(plant))
      .sort((first, second) => this.nextWateringTime(first) - this.nextWateringTime(second));
  }

  get upcomingPlants(): Plant[] {
    return this.plants
      .filter(plant => !this.plantService.isWateringOverdue(plant))
      .sort((first, second) => this.nextWateringTime(first) - this.nextWateringTime(second))
      .slice(0, 4);
  }

  loadOverview(): void {
    this.loading = true;
    this.loadError = false;

    forkJoin({
      plants: this.plantService.getAllPlants(1, 1000),
      devices: this.wateringDeviceService.getAll(1, 1000)
    }).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: ({ plants, devices }) => {
        this.plants = plants.data;
        this.devicePlantIds = new Set(
          devices.data.flatMap(device => device.plantId === null ? [] : [device.plantId])
        );
      },
      error: () => this.loadError = true
    });
  }

  markAsWatered(plant: Plant): void {
    if (this.wateringPlantIds.has(plant.id)) {
      return;
    }

    const start = new Date();
    this.wateringPlantIds.add(plant.id);
    this.wateringEventService.addWateringEvent({
      plantId: plant.id,
      start,
      end: new Date(start.getTime() + 10000)
    }).pipe(
      finalize(() => this.wateringPlantIds.delete(plant.id))
    ).subscribe({
      next: () => {
        plant.lastWateringDateTime = new Date();
        this.notificationService.showNonErrorSnackBar(`${plant.name} marked as watered`);
      },
      error: () => this.notificationService.showClientError(`Could not update ${plant.name}`)
    });
  }

  private nextWateringTime(plant: Plant): number {
    return this.plantService.getNextWateringDate(plant)?.getTime() ?? Number.NEGATIVE_INFINITY;
  }
}
