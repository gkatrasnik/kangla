import { Component, Inject } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogData } from '../../interfaces/dialog-data';


@Component({
    selector: 'app-confirm-dialog',
    imports: [MatDialogModule, MatButtonModule],
    templateUrl: './confirm-dialog.component.html',
    styleUrl: './confirm-dialog.component.scss'
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef:MatDialogRef<ConfirmDialogComponent>, 
    @Inject (MAT_DIALOG_DATA) public data: DialogData
  ) { }

  onConfirmClick(): void {
    this.dialogRef.close(true); 
  }

  onCancelClick(): void {
    this.dialogRef.close(false);
  }
}
