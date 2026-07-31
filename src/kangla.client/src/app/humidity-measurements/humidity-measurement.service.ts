import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResponse } from '../shared/interfaces/paged-response';
import { HumidityMeasurement } from './humidity-measurement';

@Injectable({ providedIn: 'root' })
export class HumidityMeasurementService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAll(deviceId: number, pageNumber: number, pageSize: number): Observable<PagedResponse<HumidityMeasurement>> {
    return this.http.get<PagedResponse<HumidityMeasurement>>(`${this.apiUrl}/HumidityMeasurements/device/${deviceId}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}
