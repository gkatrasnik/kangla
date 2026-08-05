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
  let notificationClosed: Subject<void>;

  beforeEach(() => {
    dialogOpen = jasmine.createSpy('dialogOpen').and.returnValue({ afterClosed: () => of(undefined) });
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
          useValue: { recognizePlant: () => of({ error: 'No plant was found.' }) }
        },
        { provide: LoadingService, useValue: { loadingOn: () => {}, loadingOff: () => {} } },
        {
          provide: NotificationService,
          useValue: {
            showServerError: () => ({ afterClosed: () => notificationClosed }),
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
});
