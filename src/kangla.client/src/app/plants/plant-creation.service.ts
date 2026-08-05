import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EMPTY, Observable, catchError, filter, finalize, from, of, switchMap, tap } from 'rxjs';
import { LoadingService } from '../core/loading/loading.service';
import { NotificationService } from '../core/notifications/notification.service';
import { ImagesService } from '../shared/services/images.service';
import { AddPlantDialogComponent } from './components/add-plant-dialog/add-plant-dialog.component';
import { PlantCreateRequestDto } from './dto/plant-create-request-dto';
import { PlantRecognizeResponseDto } from './dto/plant-recognize-response-dto';
import { Plant } from './plant';
import { PlantService } from './plant.service';

@Injectable({ providedIn: 'root' })
export class PlantCreationService {
  constructor(
    private dialog: MatDialog,
    private imagesService: ImagesService,
    private loadingService: LoadingService,
    private notificationService: NotificationService,
    private plantService: PlantService
  ) {}

  addManually(): Observable<Plant> {
    return this.openDialog({});
  }

  identify(file: File): Observable<Plant> {
    this.loadingService.loadingOn('Identifying plant…');

    const recognition$ = from(this.imagesService.resizeImage(file, 512, 512)).pipe(
      switchMap(resizedFile => {
        const formData = new FormData();
        formData.append('image', resizedFile);
        return this.plantService.recognizePlant(formData);
      }),
      catchError(() => {
        this.notificationService.showClientError('The plant could not be identified. You can add it manually.');
        return of({} as PlantRecognizeResponseDto);
      }),
      finalize(() => this.loadingService.loadingOff())
    );

    return recognition$.pipe(switchMap(result => {
      if (!result.error) {
        return this.openDialog(result);
      }

      return this.notificationService.showServerError(
        'Identification incomplete',
        `${result.error} You can complete the plant details manually.`
      ).afterClosed().pipe(
        switchMap(() => this.openDialog(result))
      );
    }));
  }

  private openDialog(data: PlantRecognizeResponseDto): Observable<Plant> {
    return this.dialog.open(AddPlantDialogComponent, {
      data,
      width: '36rem',
      maxWidth: 'calc(100vw - 2rem)'
    }).afterClosed().pipe(
      filter((request): request is PlantCreateRequestDto => !!request),
      switchMap(request => this.plantService.addPlant(request).pipe(
        tap(plant => this.notificationService.showNonErrorSnackBar(`${plant.name} added`)),
        catchError(() => {
          this.notificationService.showClientError('Could not add the plant');
          return EMPTY;
        })
      ))
    );
  }
}
