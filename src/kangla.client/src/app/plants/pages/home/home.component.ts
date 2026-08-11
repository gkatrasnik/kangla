import { Component, DestroyRef, OnInit, inject } from '@angular/core';
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
import { RealtimeUpdatesService } from '../../../core/realtime/realtime-updates.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  ActiveWateringCommandStatus,
  getActiveWateringCommandLabel,
  isActiveWateringCommandStatus,
  WateringCommand
} from '../../../watering-commands/watering-command';
import { WateringCommandStatusBadgeComponent } from '../../../watering-commands/components/watering-command-status-badge/watering-command-status-badge.component';
import { SoilMoistureGaugeComponent } from '../../../shared/components/soil-moisture-gauge/soil-moisture-gauge.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, ImageSrcDirective, WateringCommandStatusBadgeComponent, SoilMoistureGaugeComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
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
    private realtimeUpdatesService: RealtimeUpdatesService,
  ) {}

  ngOnInit(): void {
    this.loadHome();
    this.realtimeUpdatesService.changes$.pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(change => {
      if (change.plantId !== null && change.resources.includes('plant')) {
        this.refreshPlant(change.plantId);
      }
      if (change.deviceId !== null && (change.resources.includes('wateringCommands') || change.resources.includes('humidityMeasurements'))) {
        this.refreshWateringDevice(change.deviceId);
      }
    });
    this.realtimeUpdatesService.resync$.pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => this.loadHome());
  }

  get overduePlants(): Plant[] {
    return this.plants
      .filter(plant => this.plantService.isWateringNeeded(plant, this.wateringDevicesByPlantId.get(plant.id) ?? null))
      .sort((first, second) => this.nextWateringTime(first) - this.nextWateringTime(second));
  }

  get upcomingPlants(): Plant[] {
    return this.plants
      .filter(plant => !this.plantService.isWateringNeeded(plant, this.wateringDevicesByPlantId.get(plant.id) ?? null))
      .sort((first, second) => this.nextWateringTime(first) - this.nextWateringTime(second))
      .slice(0, 4);
  }

  loadHome(): void {
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
    if (!device || this.deviceCommandPlantIds.has(plant.id) || isActiveWateringCommandStatus(device.activeWateringCommandStatus)) {
      return;
    }

    this.deviceCommandPlantIds.add(plant.id);
    this.deviceWateringActionService.send(device, plant.name).pipe(
      finalize(() => this.deviceCommandPlantIds.delete(plant.id))
    ).subscribe(command => this.setActiveWateringCommand(command));
  }

  getActiveWateringCommandStatus(plantId: number): ActiveWateringCommandStatus | null {
    return this.wateringDevicesByPlantId.get(plantId)?.activeWateringCommandStatus ?? null;
  }

  hasActiveWateringCommand(plantId: number): boolean {
    return isActiveWateringCommandStatus(this.getActiveWateringCommandStatus(plantId));
  }

  getDeviceWateringActionLabel(plantId: number): string {
    const status = this.getActiveWateringCommandStatus(plantId);
    if (status) {
      return getActiveWateringCommandLabel(status);
    }

    return this.deviceCommandPlantIds.has(plantId) ? 'Sending…' : 'Water with device';
  }

  private nextWateringTime(plant: Plant): number {
    return this.plantService.getNextWateringDate(plant)?.getTime() ?? Number.NEGATIVE_INFINITY;
  }

  private refreshPlant(plantId: number): void {
    if (!this.plants.some(plant => plant.id === plantId)) {
      return;
    }

    this.plantService.getPlantById(plantId).subscribe({
      next: updatedPlant => {
        this.plants = this.plants.map(plant => plant.id === plantId ? updatedPlant : plant);
      },
      error: () => {}
    });
  }

  private refreshWateringDevice(deviceId: number): void {
    if (![...this.wateringDevicesByPlantId.values()].some(device => device.id === deviceId)) {
      return;
    }

    this.wateringDeviceService.get(deviceId).subscribe({
      next: device => this.replaceWateringDevice(device),
      error: () => {}
    });
  }

  private setActiveWateringCommand(command: WateringCommand): void {
    const device = [...this.wateringDevicesByPlantId.values()].find(candidate => candidate.id === command.deviceId);
    if (!device || !isActiveWateringCommandStatus(command.status)) {
      return;
    }

    this.replaceWateringDevice({ ...device, activeWateringCommandStatus: command.status });
  }

  private replaceWateringDevice(device: WateringDevice): void {
    const devices = new Map(this.wateringDevicesByPlantId);
    for (const [plantId, currentDevice] of devices) {
      if (currentDevice.id === device.id) {
        devices.delete(plantId);
      }
    }
    if (device.plantId !== null) {
      devices.set(device.plantId, device);
    }
    this.wateringDevicesByPlantId = devices;
  }
}
