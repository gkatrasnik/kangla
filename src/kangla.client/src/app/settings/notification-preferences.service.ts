import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { NotificationPreferences } from './notification-preferences';

@Injectable({ providedIn: 'root' })
export class NotificationPreferencesService {
  private readonly apiUrl = `${environment.apiUrl}/notification-preferences`;

  constructor(private readonly http: HttpClient) {}

  get(): Observable<NotificationPreferences> {
    return this.http.get<NotificationPreferences>(this.apiUrl);
  }

  setWateringReminderEmailsEnabled(enabled: boolean): Observable<NotificationPreferences> {
    return this.http.put<NotificationPreferences>(`${this.apiUrl}/watering-reminder-emails`, { enabled });
  }
}
