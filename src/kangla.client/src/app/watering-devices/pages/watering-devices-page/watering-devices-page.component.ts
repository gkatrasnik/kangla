import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Plant } from '../../../plants/plant';
import { PlantService } from '../../../plants/plant.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { DialogData } from '../../../shared/interfaces/dialog-data';
import { AddWateringDeviceDialogComponent } from '../../components/add-watering-device-dialog/add-watering-device-dialog.component';
import { WateringDevice } from '../../watering-device';
import { WateringDeviceCreateRequest, WateringDeviceService, WateringDeviceUpdateRequest } from '../../watering-device.service';

@Component({
  selector: 'app-watering-devices-page',
  standalone: true,
  imports: [RouterLink, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './watering-devices-page.component.html',
  styleUrl: './watering-devices-page.component.scss'
})
export class WateringDevicesPageComponent implements OnInit {
  devices: WateringDevice[] = [];
  plants: Plant[] = [];

  constructor(
    private wateringDeviceService: WateringDeviceService,
    private plantService: PlantService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    forkJoin({
      devices: this.wateringDeviceService.getAll(1, 1000),
      plants: this.plantService.getAllPlants(1, 1000)
    }).subscribe(({ devices, plants }) => {
      this.devices = devices.data;
      this.plants = plants.data;
    });
  }

  openAttachDialog(): void {
    this.dialog.open(AddWateringDeviceDialogComponent, { data: { plants: this.plants } })
      .afterClosed()
      .subscribe((request?: WateringDeviceCreateRequest) => {
        if (!request) {
          return;
        }

        this.wateringDeviceService.claim(request).subscribe({
          next: () => {
            this.notificationService.showNonErrorSnackBar('Watering device added to inventory');
            this.load();
          },
          error: () => this.notificationService.showClientError('Could not add watering device. Check the access key and plant selection.')
        });
      });
  }

  save(device: WateringDevice): void {
    const request: WateringDeviceUpdateRequest = {
      plantId: device.plantId,
      minimumSoilHumidity: device.minimumSoilHumidity,
      wateringIntervalSetting: device.wateringIntervalSetting,
      wateringDurationSetting: device.wateringDurationSetting
    };

    this.wateringDeviceService.update(device.id, request).subscribe({
      next: () => {
        this.notificationService.showNonErrorSnackBar('Watering device updated');
        this.load();
      },
      error: () => {
        this.notificationService.showClientError('Could not update watering device. The selected plant may already have a device, or this device may currently be watering.');
        this.load();
      }
    });
  }

  detach(device: WateringDevice): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Detach watering device',
        message: 'Detach this device from its plant? It will remain in your device inventory. A pending watering command will be cancelled; a watering device currently in progress cannot be detached.',
        confirmAction: 'Detach'
      } as DialogData
    }).afterClosed().subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.wateringDeviceService.detach(device.id).subscribe({
        next: () => {
          this.notificationService.showNonErrorSnackBar('Watering device detached');
          this.load();
        },
        error: () => this.notificationService.showClientError('Could not detach watering device. It may currently be watering.')
      });
    });
  }

  deleteFromInventory(device: WateringDevice): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Remove watering device from inventory',
        message: 'Remove this device from your inventory? A pending watering command will be cancelled. A device currently watering cannot be removed. Its command and measurement history will be retained.',
        confirmAction: 'Remove from Inventory'
      } as DialogData
    }).afterClosed().subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.wateringDeviceService.delete(device.id).subscribe({
        next: () => {
          this.notificationService.showNonErrorSnackBar('Watering device removed from inventory');
          this.load();
        },
        error: () => this.notificationService.showClientError('Could not remove watering device. It may currently be watering.')
      });
    });
  }

  isAssignedToAnotherDevice(plantId: number, currentDeviceId: number): boolean {
    return this.devices.some(device => device.id !== currentDeviceId && device.plantId === plantId);
  }
}
