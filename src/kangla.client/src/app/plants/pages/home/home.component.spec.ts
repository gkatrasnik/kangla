import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject, of } from 'rxjs';
import { ClientStateChange } from '../../../core/realtime/client-state-change';
import { RealtimeUpdatesService } from '../../../core/realtime/realtime-updates.service';
import { ImagesService } from '../../../shared/services/images.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { Plant } from '../../plant';
import { PlantCreationService } from '../../plant-creation.service';
import { PlantService } from '../../plant.service';
import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let changes: Subject<ClientStateChange>;
  let resync: Subject<void>;
  let getPlantById: jasmine.Spy;
  let getWateringDevice: jasmine.Spy;

  beforeEach(async () => {
    changes = new Subject<ClientStateChange>();
    resync = new Subject<void>();
    getPlantById = jasmine.createSpy('getPlantById');
    getWateringDevice = jasmine.createSpy('getWateringDevice');

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        {
          provide: PlantService,
          useValue: {
            getAllPlants: () => of({ pageNumber: 1, pageSize: 9, totalPages: 0, totalRecords: 0, data: [] }),
            getPlantById
          }
        },
        {
          provide: WateringDeviceService,
          useValue: {
            getAll: () => of({ pageNumber: 1, pageSize: 1000, totalPages: 0, totalRecords: 0, data: [] }),
            get: getWateringDevice
          }
        },
        { provide: ImagesService, useValue: { getImageUrl: () => undefined } },
        { provide: PlantCreationService, useValue: {} },
        { provide: RealtimeUpdatesService, useValue: { changes$: changes, resync$: resync } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('replaces an affected plant on a realtime plant change', () => {
    const original = createPlant('Needs water');
    const updated = { ...original, name: 'Updated plant', lastWateringDateTime: new Date() };
    component.plantsList = [original];
    getPlantById.and.returnValue(of(updated));

    changes.next({
      plantId: original.id,
      deviceId: 10,
      resources: ['plant'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.plantsList).toEqual([updated]);
  });

  it('refreshes an affected card device on a watering-command change', () => {
    const device = createDevice('pending');
    const updated = { ...device, activeWateringCommandStatus: 'acknowledged' as const };
    component.wateringDevicesByPlantId = new Map([[1, device]]);
    getWateringDevice.and.returnValue(of(updated));

    changes.next({
      plantId: 1,
      deviceId: device.id,
      resources: ['wateringCommands'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.wateringDevicesByPlantId.get(1)).toEqual(updated);
  });

  it('sets pending state immediately after a card creates a command', () => {
    const device = createDevice(null);
    component.wateringDevicesByPlantId = new Map([[1, device]]);

    component.onWateringCommandCreated({
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
    });

    expect(component.wateringDevicesByPlantId.get(1)?.activeWateringCommandStatus).toBe('pending');
  });

  function createPlant(name: string): Plant {
    return {
      id: 1,
      name,
      wateringInterval: 7,
      createdAt: new Date(),
      updatedAt: new Date()
    };
  }

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
