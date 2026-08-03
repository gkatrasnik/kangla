import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EMPTY, Observable, catchError, filter, switchMap, tap } from 'rxjs';
import { NotificationService } from '../core/notifications/notification.service';
import { ConfirmDialogComponent } from '../shared/components/confirm-dialog/confirm-dialog.component';
import { DialogData } from '../shared/interfaces/dialog-data';
import { WateringDevice } from '../watering-devices/watering-device';
import { WateringCommand } from './watering-command';
import { WateringCommandService } from './watering-command.service';

@Injectable({ providedIn: 'root' })
export class DeviceWateringActionService {
  constructor(
    private dialog: MatDialog,
    private notificationService: NotificationService,
    private wateringCommandService: WateringCommandService
  ) {}

  send(device: WateringDevice, plantName: string): Observable<WateringCommand> {
    return this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Send watering command?',
        message: `The pump for ${plantName} will run for ${device.wateringDurationSetting} seconds. Make sure the reservoir and tubing are ready.`,
        confirmAction: 'Send command',
        tone: 'primary'
      } as DialogData
    }).afterClosed().pipe(
      filter(Boolean),
      switchMap(() => this.wateringCommandService.create(device.id)),
      tap(() => this.notificationService.showNonErrorSnackBar(`Watering command sent to ${plantName}`)),
      catchError(() => {
        this.notificationService.showClientError('Could not send the watering command');
        return EMPTY;
      })
    );
  }
}
