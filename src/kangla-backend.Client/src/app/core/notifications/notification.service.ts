import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationDialogComponent } from '../../shared/components/notification-dialog/notification-dialog.component'

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private dialog: MatDialog, private snackbar: MatSnackBar) {}

  showServerError(title: string, message: string) {
    this.dialog.open(NotificationDialogComponent, {
      data: { title, message }
    });
  }

  showClientError(message: string): void {
    this.snackbar.open(`Error: ${message}`, 'OK', {
    });
  }

  showNonErrorSnackBar(message: string, duration = 5000) {
    this.snackbar.open(message, 'OK', {
      duration,
    });
  }
}
