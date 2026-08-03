import { Component, inject } from '@angular/core';
import { PlantService } from '../../plant.service';
import { ActivatedRoute } from '@angular/router';
import { Plant } from '../../plant';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { Location } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { EditPlantDialogComponent } from '../../components/edit-plant-dialog/edit-plant-dialog.component';
import { ConfirmDialogComponent } from  '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { DialogData } from '../../../shared/interfaces/dialog-data';
import { ImagesService } from '../../../shared/services/images.service';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';
import { WateringEventsTableComponent } from '../../../watering-events/components/watering-events-table/watering-events-table.component';
import { MatCardModule } from '@angular/material/card';
import { NotificationService } from '../../../core/notifications/notification.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { AttachInventoryDeviceDialogComponent } from '../../../watering-devices/components/attach-inventory-device-dialog/attach-inventory-device-dialog.component';
import { WateringCommandsTableComponent } from '../../../watering-commands/components/watering-commands-table/watering-commands-table.component';
import { HumidityMeasurementsTableComponent } from '../../../humidity-measurements/components/humidity-measurements-table/humidity-measurements-table.component';
import { MatMenuModule } from '@angular/material/menu';
import { DatePipe } from '@angular/common';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [ 
    MatButtonModule, 
    MatIconModule, 
    MatMenuModule,
    RouterLink,
    DatePipe,
    ImageSrcDirective, 
    WateringEventsTableComponent, 
    MatCardModule,
    WateringCommandsTableComponent,
    HumidityMeasurementsTableComponent
  ],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})

export class DetailsComponent {
  route: ActivatedRoute = inject(ActivatedRoute);
  plantId = -1;
  plant: Plant | undefined;
  wateringButtonDisabled = false;
  deviceWateringButtonDisabled = false;
  wateringDevice: WateringDevice | null = null;
  reloadTrigger = 0;
  deviceActivityReloadTrigger = 0;
  loading = true;
  loadError = false;

  constructor(
    private router: Router, 
    private location: Location,
    public plantService: PlantService,
    public imagesService: ImagesService,
    private wateringActionService: PlantWateringActionService,
    private wateringDeviceService: WateringDeviceService,
    private deviceWateringActionService: DeviceWateringActionService,
    private notificationService: NotificationService,
    public dialog: MatDialog
  ) {
    this.plantId = Number(this.route.snapshot.params['id']);
  }

  ngOnInit(): void {
    this.loadPlant();
  }

  loadPlant(): void {
    this.loading = true;
    this.loadError = false;

    forkJoin({
      plant: this.plantService.getPlantById(this.plantId),
      device: this.wateringDeviceService.getByPlantId(this.plantId).pipe(catchError(() => of(null)))
    }).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: ({ plant, device }) => {
        this.plant = plant;
        this.wateringDevice = device;
      },
      error: () => this.loadError = true
    });
  }

  removePlant(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Remove Plant',
        message: 'Are you sure you want to remove this plant?'
      } as DialogData
    });         

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Plant removed:', result);
        this.plantService.removePlant(this.plantId).subscribe(() => {
          this.router.navigate(['/home'], { replaceUrl: true });          
        });        
      }
    });
  }

  editPlant(): void {
    const dialogRef = this.dialog.open(EditPlantDialogComponent, {      
      data: this.plant,
      width: '36rem',
      maxWidth: 'calc(100vw - 2rem)'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.plantService.updatePlant(this.plantId, result).subscribe((updatedPlant: Plant) => {
          this.plant = updatedPlant;
        });
      }
    });
  }

  goBack() {
    this.location.back();
  } 

  triggerWatering(): void {
    if (!this.plant) {
      return;
    }

    this.wateringButtonDisabled = true;
    this.wateringActionService.markAsWatered(this.plant, () => this.reloadTrigger++).pipe(
      finalize(() => this.wateringButtonDisabled = false)
    ).subscribe({
      error: () => {
        this.notificationService.showClientError(`Could not update ${this.plant!.name}`);
      }
    });
  }

  triggerDeviceWatering(): void {
    if (!this.wateringDevice || !this.plant || this.deviceWateringButtonDisabled) {
      return;
    }

    this.deviceWateringButtonDisabled = true;
    this.deviceWateringActionService.send(this.wateringDevice, this.plant.name).pipe(
      finalize(() => this.deviceWateringButtonDisabled = false)
    ).subscribe(() => this.deviceActivityReloadTrigger++);
  }

  attachWateringDevice(): void {
    this.wateringDeviceService.getAll(1, 1000).subscribe(devicesResponse => {
      const availableDevices = devicesResponse.data.filter(device => device.plantId === null);
      if (availableDevices.length === 0) {
        this.notificationService.showClientError('No unattached watering devices are available in your inventory. Add one from Watering Devices first.');
        return;
      }

      this.dialog.open(AttachInventoryDeviceDialogComponent, { data: availableDevices })
        .afterClosed().subscribe((deviceId?: number) => {
          const device = availableDevices.find(availableDevice => availableDevice.id === deviceId);
          if (!device) {
            return;
          }

          this.wateringDeviceService.update(device.id, {
            plantId: this.plantId,
            minimumSoilHumidity: device.minimumSoilHumidity,
            wateringIntervalSetting: device.wateringIntervalSetting,
            wateringDurationSetting: device.wateringDurationSetting
          }).subscribe({
            next: updatedDevice => {
              this.wateringDevice = updatedDevice;
              this.notificationService.showNonErrorSnackBar('Watering device attached');
            },
            error: () => this.notificationService.showClientError('Could not attach watering device')
          });
        });
    });
  }

  detachWateringDevice(): void {
    if (!this.wateringDevice) {
      return;
    }

    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Detach watering device',
        message: 'Detach this device from the plant? It will remain in your watering-device inventory. A pending watering command will be cancelled; a watering device currently in progress cannot be detached.',
        confirmAction: 'Detach'
      } as DialogData
    }).afterClosed().subscribe(confirmed => {
      if (!confirmed || !this.wateringDevice) {
        return;
      }

      this.wateringDeviceService.detach(this.wateringDevice.id).subscribe({
        next: () => {
          this.wateringDevice = null;
          this.notificationService.showNonErrorSnackBar('Watering device detached');
        },
        error: () => this.notificationService.showClientError('Could not detach watering device. It may currently be watering.')
      });
    });
  }

  isWateringOverdue(): boolean {
    if (!this.plant) {
      return true;
    }
    return this.plantService.isWateringOverdue(this.plant);
  }

}
