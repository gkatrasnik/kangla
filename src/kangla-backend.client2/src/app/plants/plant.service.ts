import { Injectable } from '@angular/core';
import { Plant } from './plant';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PlantResponseDto } from './dto/plant-response-dto';
import { PagedResponse } from '../shared/interfaces/paged-response';
import { environment } from '../../environments/environment';
import { PlantRecognizeResponseDto } from './dto/plant-recognize-response-dto';
import { HttpContext } from '@angular/common/http';
import { SkipLoading } from '../core/loading/loading.interceptor';

@Injectable({
  providedIn: 'root'
})

export class PlantService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAllPlants(pageNumber: number, pageSize: number): Observable<PagedResponse<Plant>> {
    return this.http.get<PagedResponse<PlantResponseDto>>(`${this.apiUrl}/Plants?pageNumber=${pageNumber}&pageSize=${pageSize}`)
    .pipe(
      map(response => ({
        pageNumber: response.pageNumber,
        pageSize: response.pageSize,
        totalPages: response.totalPages,
        totalRecords: response.totalRecords,
        data: response.data.map(this.mapPlantResponseDtoToPlant)
      }))
    );
  }

  getPlantById(id: number): Observable<Plant> {
    return this.http.get<PlantResponseDto>(`${this.apiUrl}/Plants/${id}`)
    .pipe(
      map(this.mapPlantResponseDtoToPlant)
    );
  }

  addPlant(plantData: FormData) :Observable<Plant> {
    return this.http.post<PlantResponseDto>(`${this.apiUrl}/Plants`, plantData).pipe(
      map(this.mapPlantResponseDtoToPlant)
    );
  }

  removePlant(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Plants/${id}`)
  }

  updatePlant(id: number, plantData: FormData): Observable<Plant> {
    return this.http.put<PlantResponseDto>(`${this.apiUrl}/Plants/${id}`, plantData).pipe(
      map(this.mapPlantResponseDtoToPlant)
    );
  }

  recognizePlant(plantImage: FormData): Observable<PlantRecognizeResponseDto> {
    const context = new HttpContext().set(SkipLoading, true);
    return this.http.post<PlantRecognizeResponseDto>(`${this.apiUrl}/Plants/Recognize`, plantImage, {context})
  }

  private mapPlantResponseDtoToPlant(dto: PlantResponseDto): Plant {
    return {
        id: dto.id,
        name: dto.name,
        scientificName: dto.scientificName,
        description: dto.description,
        location: dto.location,
        notes: dto.notes,
        wateringInterval: dto.wateringInterval,
        wateringInstructions: dto.wateringInstructions,
        createdAt: dto.createdAt,
        updatedAt: dto.updatedAt,
        imageId: dto.imageId,
        lastWateringDateTime: dto.lastWateringDateTime
    };
  }

  isWateringOverdue(plant: Plant): boolean {
    if (!plant.lastWateringDateTime) {
        // If there's no last watering date, consider it overdue
        return true;
    }

    const lastWateredDate = new Date(plant.lastWateringDateTime);
    const currentDate = new Date();

    const nextWateringDate = new Date(lastWateredDate);
    nextWateringDate.setDate(nextWateringDate.getDate() + plant.wateringInterval);

    return currentDate > nextWateringDate;
  }
}
