import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { Subject, of } from 'rxjs';
import { LoadingService } from '../core/loading/loading.service';
import { NotificationService } from '../core/notifications/notification.service';
import { ImagesService } from '../shared/services/images.service';
import { PlantService } from './plant.service';
import { PlantCreationService } from './plant-creation.service';

describe('PlantCreationService', () => {
  let service: PlantCreationService;
  let dialogOpen: jasmine.Spy;
  let recognizePlant: jasmine.Spy;
  let showServerError: jasmine.Spy;
  let notificationClosed: Subject<void>;

  beforeEach(() => {
    dialogOpen = jasmine.createSpy('dialogOpen').and.returnValue({ afterClosed: () => of(undefined) });
    recognizePlant = jasmine.createSpy('recognizePlant').and.returnValue(of({ error: 'No plant was found.' }));
    showServerError = jasmine.createSpy('showServerError').and.returnValue({ afterClosed: () => notificationClosed });
    notificationClosed = new Subject<void>();

    TestBed.configureTestingModule({
      providers: [
        PlantCreationService,
        { provide: MatDialog, useValue: { open: dialogOpen } },
        {
          provide: ImagesService,
          useValue: { resizeImage: (file: File) => Promise.resolve(file) }
        },
        {
          provide: PlantService,
          useValue: { recognizePlant }
        },
        { provide: LoadingService, useValue: { loadingOn: () => {}, loadingOff: () => {} } },
        {
          provide: NotificationService,
          useValue: {
            showServerError,
            showClientError: () => {},
            showNonErrorSnackBar: () => {}
          }
        }
      ]
    });

    service = TestBed.inject(PlantCreationService);
  });

  it('waits for the incomplete-identification dialog before opening the add form', fakeAsync(() => {
    service.identify(new File(['not a plant'], 'photo.jpg', { type: 'image/jpeg' })).subscribe();
    tick();

    expect(dialogOpen).not.toHaveBeenCalled();

    notificationClosed.next();
    notificationClosed.complete();
    tick();

    expect(dialogOpen).toHaveBeenCalledTimes(1);
  }));

  it('asks for another photo and does not open the add form for low confidence', fakeAsync(() => {
    recognizePlant.and.returnValue(of({
      commonName: 'Possible plant',
      identificationConfidence: 'low',
      error: ''
    }));

    service.identify(new File(['plant'], 'photo.jpg', { type: 'image/jpeg' })).subscribe();
    tick();

    expect(showServerError).toHaveBeenCalledWith(
      'Low identification confidence',
      'We could not identify this plant confidently. Please upload a clearer photo showing the leaves, stems, and overall plant.'
    );
    expect(dialogOpen).not.toHaveBeenCalled();

    notificationClosed.next();
    notificationClosed.complete();
    tick();

    expect(dialogOpen).not.toHaveBeenCalled();
  }));

  ['medium', 'high'].forEach(confidence => {
    it(`opens the prefilled add form for ${confidence} confidence`, fakeAsync(() => {
      recognizePlant.and.returnValue(of({
        commonName: 'Monstera',
        identificationConfidence: confidence,
        error: ''
      }));

      service.identify(new File(['plant'], 'photo.jpg', { type: 'image/jpeg' })).subscribe();
      tick();

      expect(dialogOpen).toHaveBeenCalledTimes(1);
      expect(dialogOpen.calls.mostRecent().args[1].data).toEqual(jasmine.objectContaining({
        commonName: 'Monstera',
        identificationConfidence: confidence
      }));
    }));
  });
});
