import { Component, inject  } from '@angular/core';
import { NgFor } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { WateringDeviceCardComponent } from '../../components/watering-device-card/watering-device-card.component';
import { WateringDeviceService } from '../../watering-device.service';
import { WateringDevice } from '../../watering-device';
import { MatDialog } from '@angular/material/dialog';
import { AddDeviceDialogComponent } from '../../components/add-device-dialog/add-device-dialog.component';
import { PagedResponse } from '../../../shared/interfaces/paged-response';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ 
    WateringDeviceCardComponent,
    MatButtonModule,    
    NgFor
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  wateringDeviceService: WateringDeviceService = inject(WateringDeviceService);
  wateringDevicesList: WateringDevice[] = [];


  constructor(public dialog: MatDialog) { 
  }

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.wateringDeviceService.getAllWateringDevices().subscribe((response: PagedResponse<WateringDevice>) => {
      this.wateringDevicesList = response.data;
    });
  }

  addDevice(): void {
    const dialogRef = this.dialog.open(AddDeviceDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Device added:', result);        
        this.wateringDeviceService.addWateringDevice(result).subscribe((newDevice: WateringDevice) => {
          this.wateringDevicesList.push(newDevice);
        });
      }
    });
  }

  /*
  updateDevice(device: WateringDevice): void {
    if (this.selectedDevice?.id) {
      this.wateringDeviceService.updateDevice(this.selectedDevice.id, device).subscribe((updatedDevice: WateringDevice) => {
        const index = this.devices.findIndex(d => d.id === this.selectedDevice?.id);
        if (index !== -1) {
          this.devices[index] = updatedDevice;
        }
        this.selectedDevice = null; // Reset after update
      });
    }
  }

  deleteDevice(id: number): void {
    this.wateringDeviceService.deleteDevice(id).subscribe(() => {
      this.devices = this.devices.filter(d => d.id !== id);
    });
  }
    */
}
