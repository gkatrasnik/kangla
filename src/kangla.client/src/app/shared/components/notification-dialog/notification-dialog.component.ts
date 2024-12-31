import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef,  } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DialogData } from '../../interfaces/dialog-data';

@Component({
    selector: 'app-notification-dialog',
    imports: [MatButtonModule, MatDialogModule],
    templateUrl: './notification-dialog.component.html',
    styleUrl: './notification-dialog.component.scss'
})
export class NotificationDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<NotificationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  close(): void {
    this.dialogRef.close();
  }
}
