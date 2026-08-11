import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Subject, of } from 'rxjs';
import { ClientStateChange } from '../../../core/realtime/client-state-change';
import { RealtimeUpdatesService } from '../../../core/realtime/realtime-updates.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ImagesService } from '../../../shared/services/images.service';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';
import { WateringDeviceService } from '../../../watering-devices/watering-device.service';
import { WateringDevice } from '../../../watering-devices/watering-device';
import { Plant } from '../../plant';
import { PlantCreationService } from '../../plant-creation.service';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { PlantService } from '../../plant.service';
import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let changes: Subject<ClientStateChange>;
  let getPlantById: jasmine.Spy;
  let getWateringDevice: jasmine.Spy;

  beforeEach(async () => {
    changes = new Subject<ClientStateChange>();
    getPlantById = jasmine.createSpy('getPlantById');
    getWateringDevice = jasmine.createSpy('getWateringDevice');

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        provideRouter([]),
        {
          provide: PlantService,
          useValue: {
            getAllPlants: () => of({ pageNumber: 1, pageSize: 1000, totalPages: 0, totalRecords: 0, data: [] }),
            getPlantById,
            isWateringOverdue: (plant: Plant) => !plant.lastWateringDateTime,
            getNextWateringDate: () => new Date(),
            getCareStatusLabel: () => 'Water today'
          }
        },
        {
          provide: WateringDeviceService,
          useValue: {
            getAll: () => of({ pageNumber: 1, pageSize: 1000, totalPages: 0, totalRecords: 0, data: [] }),
            get: getWateringDevice
          }
        },
        { provide: PlantWateringActionService, useValue: {} },
        { provide: DeviceWateringActionService, useValue: {} },
        { provide: PlantCreationService, useValue: {} },
        { provide: NotificationService, useValue: {} },
        { provide: ImagesService, useValue: { getImageUrl: () => undefined } },
        {
          provide: RealtimeUpdatesService,
          useValue: { changes$: changes, resync$: new Subject<void>() }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('removes a watered plant from needs attention after a realtime update', () => {
    const plant: Plant = {
      id: 1,
      name: 'Needs water',
      wateringInterval: 7,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    const updated = { ...plant, lastWateringDateTime: new Date() };
    component.plants = [plant];
    getPlantById.and.returnValue(of(updated));

    changes.next({
      plantId: plant.id,
      deviceId: 10,
      resources: ['plant'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.overduePlants).toEqual([]);
  });

  it('shows active command indicators in needs-attention and coming-up sections', () => {
    const overdue = createPlant(1, undefined);
    const upcoming = createPlant(2, new Date());
    component.plants = [overdue, upcoming];
    component.wateringDevicesByPlantId = new Map([
      [overdue.id, createDevice(10, overdue.id, 'pending')],
      [upcoming.id, createDevice(11, upcoming.id, 'acknowledged')]
    ]);

    fixture.detectChanges();

    const badges = fixture.nativeElement.querySelectorAll('app-watering-command-status-badge');
    expect(badges.length).toBe(2);
    expect(fixture.nativeElement.textContent).toContain('Watering queued');
    expect(fixture.nativeElement.textContent).toContain('Watering now');
  });

  it('clears an active indicator after a realtime command update', () => {
    const device = createDevice(10, 1, 'acknowledged');
    component.wateringDevicesByPlantId = new Map([[1, device]]);
    getWateringDevice.and.returnValue(of({ ...device, activeWateringCommandStatus: null }));

    changes.next({
      plantId: 1,
      deviceId: device.id,
      resources: ['wateringCommands'],
      occurredAtUtc: new Date().toISOString()
    });

    expect(component.wateringDevicesByPlantId.get(1)?.activeWateringCommandStatus).toBeNull();
  });

  function createPlant(id: number, lastWateringDateTime: Date | undefined): Plant {
    return {
      id,
      name: `Plant ${id}`,
      wateringInterval: 7,
      lastWateringDateTime,
      createdAt: new Date(),
      updatedAt: new Date()
    };
  }

  function createDevice(
    id: number,
    plantId: number,
    status: WateringDevice['activeWateringCommandStatus']
  ): WateringDevice {
    return {
      id,
      active: true,
      deleted: false,
      minimumSoilHumidity: 400,
      wateringIntervalSetting: 7,
      wateringDurationSetting: 3,
      plantId,
      activeWateringCommandStatus: status
    };
  }
});
