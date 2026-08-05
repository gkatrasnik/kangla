import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { WateringDevice } from './watering-device';
import { PagedResponse } from '../shared/interfaces/paged-response';

export interface WateringDeviceCreateRequest {
  minimumSoilHumidity: number;
  wateringIntervalSetting: number;
  wateringDurationSetting: number;
  deviceAccessKey: string;
  plantId?: number | null;
}

export interface WateringDeviceUpdateRequest {
  minimumSoilHumidity: number;
  wateringIntervalSetting: number;
  wateringDurationSetting: number;
  plantId?: number | null;
}

@Injectable({ providedIn: 'root' })
export class WateringDeviceService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getByPlantId(plantId: number): Observable<WateringDevice> {
    return this.http.get<WateringDevice>(`${this.apiUrl}/WateringDevices/plant/${plantId}`);
  }

  get(deviceId: number): Observable<WateringDevice> {
    return this.http.get<WateringDevice>(`${this.apiUrl}/WateringDevices/device/${deviceId}`);
  }

  getAll(pageNumber: number, pageSize: number): Observable<PagedResponse<WateringDevice>> {
    return this.http.get<PagedResponse<WateringDevice>>(`${this.apiUrl}/WateringDevices?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  claim(device: WateringDeviceCreateRequest): Observable<WateringDevice> {
    return this.http.post<WateringDevice>(`${this.apiUrl}/WateringDevices`, device);
  }

  update(deviceId: number, device: WateringDeviceUpdateRequest): Observable<WateringDevice> {
    return this.http.put<WateringDevice>(`${this.apiUrl}/WateringDevices/${deviceId}`, device);
  }

  detach(deviceId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/WateringDevices/${deviceId}/plant`);
  }

  delete(deviceId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/WateringDevices/${deviceId}`);
  }
}
