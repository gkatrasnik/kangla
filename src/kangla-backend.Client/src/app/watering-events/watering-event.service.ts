import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { WateringEvent } from './watering-event';
import { WateringEventResponseDto } from './dto/watering-event-response-dto';
import { WateringEventCreateRequestDto } from './dto/watering-event-create-request-dto';
import { PagedResponse } from '../shared/interfaces/paged-response';

@Injectable({
  providedIn: 'root'
})
export class WateringEventService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAllWateringEventsByPlantId(id: number, pageNumber: number, pageSize: number): Observable<PagedResponse<WateringEvent>> {
    return this.http.get<PagedResponse<WateringEventResponseDto>>(`${this.apiUrl}/WateringEvents/plant/${id}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
    .pipe(
      map(response => ({
        pageNumber: response.pageNumber,
        pageSize: response.pageSize,
        totalPages: response.totalPages,
        totalRecords: response.totalRecords,
        data: response.data.map(this.mapWateringEventResponseDtoToWateringEvent)
      }))
    );
  }

  addWateringEvent(wateringEventCreateRequestDto: WateringEventCreateRequestDto): Observable<WateringEvent> {
    return this.http.post<WateringEventResponseDto>(`${this.apiUrl}/WateringEvents`, wateringEventCreateRequestDto)
    .pipe(
      map(this.mapWateringEventResponseDtoToWateringEvent)
    );
  }

  deleteWateringEvent(wateringEventId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/WateringEvents/${wateringEventId}`);
  }

  mapWateringEventResponseDtoToWateringEvent(wateringEventResponseDto: WateringEventResponseDto): WateringEvent {
    return {
      id: wateringEventResponseDto.id,
      plantId: wateringEventResponseDto.plantId,
      start: wateringEventResponseDto.start,
      end: wateringEventResponseDto.end,
      createdAt: wateringEventResponseDto.createdAt,
      updatedAt: wateringEventResponseDto.updatedAt
    }
  }  
}
