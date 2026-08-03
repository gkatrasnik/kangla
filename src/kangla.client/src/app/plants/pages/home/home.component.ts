import { Component } from '@angular/core';
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

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ 
    PlantCardComponent,
    MatPaginatorModule,
    MatButtonModule,    
    MatIconModule,
    MatMenuModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
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
    private plantCreationService: PlantCreationService
  ) {}

  handlePageEvent(e: PageEvent) {
    this.plantsListLength = e.length;
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.loadPlants(this.pageIndex, this.pageSize);
  }

  ngOnInit(): void {
    this.loadPlants(this.pageIndex, this.pageSize);
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
}
