import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationDialogComponent } from '../../shared/components/notification-dialog/notification-dialog.component'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private dialog: MatDialog, private snackbar: MatSnackBar) {}

  showServerError(title: string, message: string) {
    return this.dialog.open(NotificationDialogComponent, {
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

  showUndoSnackBar(message: string, duration = 7000): Observable<void> {
    return this.snackbar.open(message, 'Undo', { duration }).onAction();
  }
}
