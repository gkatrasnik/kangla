import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { WateringCommand } from './watering-command';
import { PagedResponse } from '../shared/interfaces/paged-response';

@Injectable({ providedIn: 'root' })
export class WateringCommandService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  create(deviceId: number): Observable<WateringCommand> {
    return this.http.post<WateringCommand>(`${this.apiUrl}/WateringDevices/${deviceId}/watering-commands`, {});
  }

  get(deviceId: number, commandId: number): Observable<WateringCommand> {
    return this.http.get<WateringCommand>(`${this.apiUrl}/WateringDevices/${deviceId}/watering-commands/${commandId}`);
  }

  getAll(deviceId: number, pageNumber: number, pageSize: number): Observable<PagedResponse<WateringCommand>> {
    return this.http.get<PagedResponse<WateringCommand>>(`${this.apiUrl}/WateringDevices/${deviceId}/watering-commands?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}
