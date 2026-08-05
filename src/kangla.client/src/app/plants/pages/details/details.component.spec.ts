import { Location } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Subject, of } from 'rxjs';
import { ClientStateChange } from '../../../core/realtime/client-state-change';
import { RealtimeUpdatesService } from '../../../core/realtime/realtime-updates.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ImagesService } from '../../../shared/services/images.service';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';
import { WateringEventService } from '../../../watering-events/watering-event.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { WateringCommandService } from '../../../watering-commands/watering-command.service';
import { HumidityMeasurementService } from '../../../humidity-measurements/humidity-measurement.service';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { PlantService } from '../../plant.service';
import { DetailsComponent } from './details.component';

describe('DetailsComponent', () => {
  let component: DetailsComponent;
  let fixture: ComponentFixture<DetailsComponent>;
  let changes: Subject<ClientStateChange>;
  let getWateringDevice: jasmine.Spy;
  let sendWateringCommand: jasmine.Spy;

  beforeEach(async () => {
    changes = new Subject<ClientStateChange>();
    getWateringDevice = jasmine.createSpy('getWateringDevice');
    sendWateringCommand = jasmine.createSpy('sendWateringCommand');

    await TestBed.configureTestingModule({
      imports: [DetailsComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { params: { id: '1' } } } },
        {
          provide: PlantService,
          useValue: {
            getPlantById: () => of({
              id: 1,
              name: 'Test plant',
              wateringInterval: 7,
              createdAt: new Date(),
              updatedAt: new Date()
            }),
            isWateringOverdue: () => false,
            getCareStatusLabel: () => 'Water in 7 days',
            getNextWateringDate: () => new Date()
          }
        },
        {
          provide: WateringDeviceService,
          useValue: { getByPlantId: () => of(null), get: getWateringDevice }
        },
        { provide: PlantWateringActionService, useValue: {} },
        { provide: DeviceWateringActionService, useValue: { send: sendWateringCommand } },
        {
          provide: WateringEventService,
          useValue: {
            getAllWateringEventsByPlantId: () => of({
              pageNumber: 1,
              pageSize: 10,
              totalPages: 0,
              totalRecords: 0,
              data: []
            })
          }
        },
        {
          provide: WateringCommandService,
          useValue: {
            getAll: () => of({ pageNumber: 1, pageSize: 10, totalPages: 0, totalRecords: 0, data: [] })
          }
        },
        {
          provide: HumidityMeasurementService,
          useValue: {
            getAll: () => of({ pageNumber: 1, pageSize: 10, totalPages: 0, totalRecords: 0, data: [] })
          }
        },
        { provide: NotificationService, useValue: {} },
        { provide: ImagesService, useValue: { getImageUrl: () => undefined } },
        { provide: MatDialog, useValue: {} },
        { provide: Location, useValue: { back: () => {} } },
        { provide: RealtimeUpdatesService, useValue: { changes$: changes, resync$: new Subject<void>() } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('refreshes only the affected activity resources', () => {
    changes.next({
      plantId: 1,
      deviceId: 10,
      resources: ['wateringCommands', 'humidityMeasurements', 'wateringEvents'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.wateringCommandsReloadTrigger).toBe(1);
    expect(component.humidityMeasurementsReloadTrigger).toBe(1);
    expect(component.reloadTrigger).toBe(1);
  });

  it('clears the hero command status after a realtime terminal update', () => {
    const device = createDevice('acknowledged');
    component.wateringDevice = device;
    getWateringDevice.and.returnValue(of({ ...device, activeWateringCommandStatus: null }));

    changes.next({
      plantId: 1,
      deviceId: device.id,
      resources: ['wateringCommands'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.wateringDevice?.activeWateringCommandStatus).toBeNull();
  });

  it('sets pending status immediately after creating a command', () => {
    const device = createDevice(null);
    component.wateringDevice = device;
    sendWateringCommand.and.returnValue(of({
      id: 20,
      deviceId: device.id,
      status: 'pending',
      durationSeconds: 3,
      requestedAtUtc: new Date().toISOString(),
      expiresAtUtc: new Date().toISOString(),
      acknowledgedAtUtc: null,
      startedAtUtc: null,
      finishedAtUtc: null,
      failureReason: null,
      wateringEventId: null
    }));

    component.triggerDeviceWatering();

    expect(component.wateringDevice?.activeWateringCommandStatus).toBe('pending');
  });

  it('shows the active command in the details hero and disables device watering', () => {
    component.wateringDevice = createDevice('acknowledged');

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Watering now');
    const wateringButton = [...fixture.nativeElement.querySelectorAll('.hero-actions button')]
      .find((button: HTMLButtonElement) => button.textContent?.includes('Watering now')) as HTMLButtonElement;
    expect(wateringButton.disabled).toBeTrue();
  });

  function createDevice(status: WateringDevice['activeWateringCommandStatus']): WateringDevice {
    return {
      id: 10,
      active: true,
      deleted: false,
      minimumSoilHumidity: 400,
      wateringIntervalSetting: 7,
      wateringDurationSetting: 3,
      plantId: 1,
      activeWateringCommandStatus: status
    };
  }
});
