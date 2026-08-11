import { Component, DestroyRef, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PlantCardComponent } from '../../components/plant-card/plant-card.component';
import { PlantService } from '../../plant.service';
import { Plant } from '../../plant';
import { ImagesService } from '../../../shared/services/images.service';
import { MatIconModule } from '@angular/material/icon';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { finalize, forkJoin } from 'rxjs';
import { MatMenuModule } from '@angular/material/menu';
import { PlantCreationService } from '../../plant-creation.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { RealtimeUpdatesService } from '../../../core/realtime/realtime-updates.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { WateringCommand } from '../../../watering-commands/watering-command';

@Component({
  selector: 'app-plants',
  standalone: true,
  imports: [ 
    PlantCardComponent,
    MatPaginatorModule,
    MatButtonModule,    
    MatIconModule,
    MatMenuModule
  ],
  templateUrl: './plants.component.html',
  styleUrl: './plants.component.scss'
})
export class PlantsComponent {
  private readonly destroyRef = inject(DestroyRef);
  plantsList: Plant[] = [];
  wateringDevicesByPlantId = new Map<number, WateringDevice>();

  plantsListLength = 0;
  pageSize = 9;
  pageIndex = 0;
  pageSizeOptions = [9, 15, 30];

  hidePageSize = false;
  showPageSizeOptions = true;
  showFirstLastButtons = true;
  disabled = false;
  loading = true;
  loadError = false;

  constructor(
    private plantService: PlantService,
    public imagesService: ImagesService,
    private wateringDeviceService: WateringDeviceService,
    private plantCreationService: PlantCreationService,
    private realtimeUpdatesService: RealtimeUpdatesService
  ) {}

  handlePageEvent(e: PageEvent) {
    this.plantsListLength = e.length;
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.loadPlants(this.pageIndex, this.pageSize);
  }

  ngOnInit(): void {
    this.loadPlants(this.pageIndex, this.pageSize);
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
    ).subscribe(() => this.loadPlants(this.pageIndex, this.pageSize));
  }

  loadPlants(pageIndex: number, pageSize: number): void {
    this.loading = true;
    this.loadError = false;

    forkJoin({
      plants: this.plantService.getAllPlants(pageIndex + 1, pageSize),
      devices: this.wateringDeviceService.getAll(1, 1000)
    }).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: ({ plants, devices }) => {
        this.plantsList = plants.data;
        this.plantsListLength = plants.totalRecords;
        this.wateringDevicesByPlantId = new Map(
          devices.data.flatMap(device => device.plantId === null ? [] : [[device.plantId, device] as const])
        );
      },
      error: () => this.loadError = true
    });
  }

  onImageSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];

    if (file) {
      this.plantCreationService.identify(file).subscribe(() => this.reloadPlants());
    }

    fileInput.value = '';
  }

  addPlantManually(): void {
    this.plantCreationService.addManually().subscribe(() => this.reloadPlants());
  }

  reloadPlants(): void {
    this.pageIndex = 0;
    this.loadPlants(this.pageIndex, this.pageSize);
  }

  onWateringCommandCreated(command: WateringCommand): void {
    this.setActiveWateringCommandStatus(command.deviceId, command.status);
  }

  private refreshPlant(plantId: number): void {
    if (!this.plantsList.some(plant => plant.id === plantId)) {
      return;
    }

    this.plantService.getPlantById(plantId).subscribe({
      next: updatedPlant => {
        this.plantsList = this.plantsList.map(plant => plant.id === plantId ? updatedPlant : plant);
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

  private setActiveWateringCommandStatus(deviceId: number, status: WateringCommand['status']): void {
    const device = [...this.wateringDevicesByPlantId.values()].find(candidate => candidate.id === deviceId);
    if (!device || (status !== 'pending' && status !== 'acknowledged')) {
      return;
    }

    this.replaceWateringDevice({ ...device, activeWateringCommandStatus: status });
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
