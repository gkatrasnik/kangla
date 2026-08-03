import { Component, inject, OnDestroy } from '@angular/core';
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
import { WateringEventService } from '../../../watering-events/watering-event.service';
import { WateringEventsTableComponent } from '../../../watering-events/components/watering-events-table/watering-events-table.component';
import { MatCardModule } from '@angular/material/card';
import { WateringEventCreateRequestDto } from '../../../watering-events/dto/watering-event-create-request-dto';
import { NotificationService } from '../../../core/notifications/notification.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { WateringCommandService } from '../../../watering-commands/watering-command.service';
import { WateringCommand } from '../../../watering-commands/watering-command';
import { forkJoin, of, Subscription, timer } from 'rxjs';
import { catchError, finalize, switchMap, takeWhile } from 'rxjs/operators';
import { AttachInventoryDeviceDialogComponent } from '../../../watering-devices/components/attach-inventory-device-dialog/attach-inventory-device-dialog.component';
import { WateringCommandsTableComponent } from '../../../watering-commands/components/watering-commands-table/watering-commands-table.component';
import { HumidityMeasurementsTableComponent } from '../../../humidity-measurements/components/humidity-measurements-table/humidity-measurements-table.component';
import { MatMenuModule } from '@angular/material/menu';
import { DatePipe } from '@angular/common';

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

export class DetailsComponent implements OnDestroy {
  route: ActivatedRoute = inject(ActivatedRoute);
  plantId = -1;
  plant: Plant | undefined;
  wateringButtonDisabled = false;
  deviceWateringButtonDisabled = false;
  deviceWateringStatus: string | null = null;
  wateringDevice: WateringDevice | null = null;
  reloadTrigger = 0;
  deviceActivityReloadTrigger = 0;
  private commandStatusSubscription?: Subscription;
  loading = true;
  loadError = false;

  constructor(
    private router: Router, 
    private location: Location,
    public plantService: PlantService,
    public imagesService: ImagesService,
    private wateringEventService: WateringEventService,
    private wateringDeviceService: WateringDeviceService,
    private wateringCommandService: WateringCommandService,
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

  triggerWatering() {
    const start = new Date();
    const end = new Date(start.getTime() + 10000); // End time 10 seconds after start

    if (!this.plant) {
      return;
    }

    const wateringEvent: WateringEventCreateRequestDto = {
      plantId: this.plant.id,
      start: start,
      end: end
    };

    this.wateringButtonDisabled = true;
    this.wateringEventService.addWateringEvent(wateringEvent).subscribe({
      next: (response) => {
        console.log('Watering event created:', response);
        this.notificationService.showNonErrorSnackBar(`${this.plant!.name} marked as watered`);
        this.plant!.lastWateringDateTime = new Date();
        this.reloadTrigger++;
      },
      error: () => {
        this.wateringButtonDisabled = false;
        this.notificationService.showClientError(`Could not update ${this.plant!.name}`);
      }
    });
  }

  triggerDeviceWatering(): void {
    if (!this.wateringDevice) {
      return;
    }

    this.deviceWateringButtonDisabled = true;
    this.wateringCommandService.create(this.wateringDevice.id).subscribe({
      next: command => {
        this.deviceActivityReloadTrigger++;
        this.pollDeviceWateringCommand(command);
      },
      error: () => {
        this.deviceWateringButtonDisabled = false;
        this.notificationService.showClientError('Could not request device watering');
      }
    });
  }

  private pollDeviceWateringCommand(command: WateringCommand): void {
    this.deviceWateringStatus = this.toDisplayStatus(command.status);
    this.commandStatusSubscription?.unsubscribe();
    this.commandStatusSubscription = timer(0, 5000).pipe(
      switchMap(() => this.wateringCommandService.get(command.deviceId, command.id)),
      takeWhile(current => this.isActiveCommand(current), true)
    ).subscribe({
      next: current => {
        this.deviceWateringStatus = this.toDisplayStatus(current.status);
        if (current.status === 'completed') {
          this.plant!.lastWateringDateTime = new Date(current.finishedAtUtc!);
          this.reloadTrigger++;
          this.deviceActivityReloadTrigger++;
          this.notificationService.showNonErrorSnackBar('Device watering completed');
        } else if (!this.isActiveCommand(current)) {
          this.deviceActivityReloadTrigger++;
          this.notificationService.showClientError(current.failureReason ?? `Device watering ${current.status}`);
        }

        if (!this.isActiveCommand(current)) {
          this.deviceWateringButtonDisabled = false;
        }
      },
      error: () => {
        this.deviceWateringButtonDisabled = false;
        this.notificationService.showClientError('Could not read device watering status');
      }
    });
  }

  private isActiveCommand(command: WateringCommand): boolean {
    return command.status === 'pending' || command.status === 'acknowledged';
  }

  private toDisplayStatus(status: WateringCommand['status']): string {
    return status.charAt(0).toUpperCase() + status.slice(1);
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
          this.commandStatusSubscription?.unsubscribe();
          this.wateringDevice = null;
          this.deviceWateringStatus = null;
          this.notificationService.showNonErrorSnackBar('Watering device detached');
        },
        error: () => this.notificationService.showClientError('Could not detach watering device. It may currently be watering.')
      });
    });
  }

  ngOnDestroy(): void {
    this.commandStatusSubscription?.unsubscribe();
  }

  isWateringOverdue(): boolean {
    if (!this.plant) {
      return true;
    }
    return this.plantService.isWateringOverdue(this.plant);
  }

}
