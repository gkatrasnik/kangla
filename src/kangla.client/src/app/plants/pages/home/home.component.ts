import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PlantCardComponent } from '../../components/plant-card/plant-card.component';
import { PlantService } from '../../plant.service';
import { Plant } from '../../plant';
import { MatDialog } from '@angular/material/dialog';
import { AddPlantDialogComponent } from '../../components/add-plant-dialog/add-plant-dialog.component';
import { ImagesService } from '../../../shared/services/images.service';
import { MatIconModule } from '@angular/material/icon';
import { PlantRecognizeResponseDto } from '../../dto/plant-recognize-response-dto';
import { NotificationService } from '../../../core/notifications/notification.service';
import { LoadingService } from '../../../core/loading/loading.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { finalize, forkJoin } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ 
    PlantCardComponent,
    MatPaginatorModule,
    MatButtonModule,    
    MatIconModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  plantsList: Plant[] = [];
  devicePlantIds = new Set<number>();

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
    private notificationService: NotificationService,
    private loadingService: LoadingService,
    private wateringDeviceService: WateringDeviceService,
    public dialog: MatDialog
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
        this.devicePlantIds = new Set(devices.data.flatMap(device => device.plantId === null ? [] : [device.plantId]));
      },
      error: () => this.loadError = true
    });
  }

  async onImageSelected(event: Event): Promise<void> {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];

    if (file) {
      this.loadingService.loadingOn('Recognizing plant...');

      try {
        const resizedFile = await this.imagesService.resizeImage(file, 512, 512);
        const formData = new FormData();
        formData.append('image', resizedFile);

        this.plantService.recognizePlant(formData).subscribe({
          next: (recognizedPlant: PlantRecognizeResponseDto) => {           
            this.openAddPlantDialog(recognizedPlant);

            if (recognizedPlant.error) {
              const msg = recognizedPlant.error + " You can add this plant manually.";
              this.notificationService.showServerError('Oops', msg);
            }
          },
          error: (err) => {
            this.loadingService.loadingOff();
            console.error('Plant recognition failed:', err);
            this.openAddPlantDialog();
            throw new Error('Plant recognition failed.');
          },
          complete: () => {
            this.loadingService.loadingOff();
          }
        });
      } catch (error) {
        this.loadingService.loadingOff();
        throw new Error('Error processing image.');
      } finally {
        fileInput.value = '';
      }
    }
  }

  openAddPlantDialog(plantData?: PlantRecognizeResponseDto): void {
    const dialogRef = this.dialog.open(AddPlantDialogComponent, {
      data: plantData || {},
      width: '36rem',
      maxWidth: 'calc(100vw - 2rem)'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Plant added:', result);
        this.plantService.addPlant(result).subscribe((newPlant: Plant) => {
          this.plantsList.push(newPlant);
          this.reloadPlants();
        });
      }
    });
  }

  reloadPlants(): void {
    this.pageIndex = 0;
    this.loadPlants(this.pageIndex, this.pageSize);
  }
}
