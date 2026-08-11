import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { WateringDeviceCreateRequest } from '../../watering-device.service';
import { Plant } from '../../../plants/plant';
import { MatSelectModule } from '@angular/material/select';

export interface AddWateringDeviceDialogData {
  plantId?: number;
  plants: Plant[];
}

@Component({
  selector: 'app-add-watering-device-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, ReactiveFormsModule],
  templateUrl: './add-watering-device-dialog.component.html',
  styleUrl: './add-watering-device-dialog.component.scss'
})
export class AddWateringDeviceDialogComponent {
  form: FormGroup;

  constructor(
    formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<AddWateringDeviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AddWateringDeviceDialogData
  ) {
    this.form = formBuilder.group({
      deviceAccessKey: ['', [Validators.required, Validators.maxLength(128)]],
      plantId: [data.plantId ?? null],
      wateringIntervalSetting: [7, [Validators.required, Validators.min(1), Validators.max(365)]],
      wateringDurationSetting: [3, [Validators.required, Validators.min(1), Validators.max(60)]]
    });
  }

  submit(): void {
    if (!this.form.valid) {
      return;
    }

    this.dialogRef.close(this.form.value as WateringDeviceCreateRequest);
  }
}
