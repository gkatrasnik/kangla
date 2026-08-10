import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { NotificationPreferencesService } from '../../notification-preferences.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [MatCardModule, MatSlideToggleModule, MatProgressSpinnerModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  enabled = false;
  loading = true;
  saving = false;
  loadError = false;

  constructor(private readonly preferencesService: NotificationPreferencesService, private readonly notificationService: NotificationService) {}

  ngOnInit(): void {
    this.preferencesService.get().pipe(finalize(() => this.loading = false)).subscribe({
      next: preferences => this.enabled = preferences.wateringReminderEmailsEnabled,
      error: () => this.loadError = true
    });
  }

  updateEnabled(enabled: boolean): void {
    if (this.saving) return;
    const previousValue = this.enabled;
    this.enabled = enabled;
    this.saving = true;
    this.preferencesService.setWateringReminderEmailsEnabled(enabled).pipe(finalize(() => this.saving = false)).subscribe({
      next: preferences => {
        this.enabled = preferences.wateringReminderEmailsEnabled;
        this.notificationService.showNonErrorSnackBar('Settings saved.');
      },
      error: () => { this.enabled = previousValue; this.notificationService.showClientError('Could not save your notification settings.'); }
    });
  }
}
