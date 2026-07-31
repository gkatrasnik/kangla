import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { WateringDevice } from '../../watering-device';

@Component({
  selector: 'app-attach-inventory-device-dialog',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './attach-inventory-device-dialog.component.html'
})
export class AttachInventoryDeviceDialogComponent {
  selectedDeviceId: number | null = null;

  constructor(
    private dialogRef: MatDialogRef<AttachInventoryDeviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public devices: WateringDevice[]
  ) {}

  attach(): void {
    if (this.selectedDeviceId !== null) {
      this.dialogRef.close(this.selectedDeviceId);
    }
  }
}
