import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PlantCardComponent } from '../../components/plant-card/plant-card.component';
import { PlantService } from '../../plant.service';
import { Plant } from '../../plant';
import { MatDialog } from '@angular/material/dialog';
import { AddPlantDialogComponent } from '../../components/add-plant-dialog/add-plant-dialog.component';
import { PagedResponse } from '../../../shared/interfaces/paged-response';
import { ImagesService } from '../../../shared/services/images.service';
import { MatIconModule } from '@angular/material/icon';
import { PlantRecognizeResponseDto } from '../../dto/plant-recognize-response-dto';
import { NotificationService } from '../../../core/notifications/notification.service';
import { LoadingService } from '../../../core/loading/loading.service';
import { MatMenuModule } from '@angular/material/menu';

@Component({
    selector: 'app-home',
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

  plantsListLength = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [9, 15, 30];

  hidePageSize = false;
  showPageSizeOptions = true;
  showFirstLastButtons = true;
  disabled = false;

  constructor(
    private plantService: PlantService,
    public imagesService: ImagesService,
    private notificationService: NotificationService,
    private loadingService: LoadingService,
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
    this.plantService.getAllPlants(pageIndex + 1, pageSize).subscribe((response: PagedResponse<Plant>) => {
      this.plantsList = response.data;
      this.plantsListLength = response.totalRecords;
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
            console.error('Plant recognition failed:', err);
            this.openAddPlantDialog();
            throw new Error('Plant recognition failed');
          },
          complete: () => {
            this.loadingService.loadingOff();
          }
        });
      } catch (error) {
        this.loadingService.loadingOff();
        throw new Error('Error during plant recognition');
      } finally {
        fileInput.value = '';
      }
    }
  }

  openAddPlantDialog(plantData?: PlantRecognizeResponseDto): void {
    const dialogRef = this.dialog.open(AddPlantDialogComponent, {
      data: plantData || {}
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
