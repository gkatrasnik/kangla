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
import { NotificationService } from '../../../core/notifications/notification.service';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { MatMenuModule } from '@angular/material/menu';
import { PlantCreationService } from '../../plant-creation.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, ImageSrcDirective],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.scss'
})
export class OverviewComponent implements OnInit {
  plants: Plant[] = [];
  wateringDevicesByPlantId = new Map<number, WateringDevice>();
  wateringPlantIds = new Set<number>();
  deviceCommandPlantIds = new Set<number>();
  loading = true;
  loadError = false;

  constructor(
    public plantService: PlantService,
    public imagesService: ImagesService,
    private wateringDeviceService: WateringDeviceService,
    private wateringActionService: PlantWateringActionService,
    private deviceWateringActionService: DeviceWateringActionService,
    private plantCreationService: PlantCreationService,
    private notificationService: NotificationService,
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
        this.wateringDevicesByPlantId = new Map(
          devices.data.flatMap(device => device.plantId === null ? [] : [[device.plantId, device] as const])
        );
      },
      error: () => this.loadError = true
    });
  }

  markAsWatered(plant: Plant): void {
    if (this.wateringPlantIds.has(plant.id)) {
      return;
    }

    this.wateringPlantIds.add(plant.id);
    this.wateringActionService.markAsWatered(plant).pipe(
      finalize(() => this.wateringPlantIds.delete(plant.id))
    ).subscribe({
      next: () => {},
      error: () => this.notificationService.showClientError(`Could not update ${plant.name}`)
    });
  }

  onImageSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];

    if (file) {
      this.plantCreationService.identify(file).subscribe(plant => this.plants.push(plant));
    }

    fileInput.value = '';
  }

  addPlantManually(): void {
    this.plantCreationService.addManually().subscribe(plant => this.plants.push(plant));
  }

  sendWateringCommand(plant: Plant): void {
    const device = this.wateringDevicesByPlantId.get(plant.id);
    if (!device || this.deviceCommandPlantIds.has(plant.id)) {
      return;
    }

    this.deviceCommandPlantIds.add(plant.id);
    this.deviceWateringActionService.send(device, plant.name).pipe(
      finalize(() => this.deviceCommandPlantIds.delete(plant.id))
    ).subscribe();
  }

  private nextWateringTime(plant: Plant): number {
    return this.plantService.getNextWateringDate(plant)?.getTime() ?? Number.NEGATIVE_INFINITY;
  }
}
