import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';

import { PlantService } from './plant.service';
import { Plant } from './plant';
import { WateringDevice } from '../watering-devices/watering-device';

describe('PlantService', () => {
  let service: PlantService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient()] });
    service = TestBed.inject(PlantService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('uses the inclusive 10-point moisture green zone when a device is attached', () => {
    const plant = createPlant(50);

    expect(service.isWateringNeeded(plant, createDevice(40))).toBeFalse();
    expect(service.isWateringNeeded(plant, createDevice(60))).toBeFalse();
    expect(service.isWateringNeeded(plant, createDevice(39))).toBeTrue();
    expect(service.isWateringNeeded(plant, createDevice(61))).toBeTrue();
  });

  it('uses the watering interval only when no device is attached', () => {
    const plant = createPlant(50);
    plant.lastWateringDateTime = new Date('2000-01-01T00:00:00Z');

    expect(service.isWateringNeeded(plant)).toBeTrue();
    expect(service.isWateringNeeded(plant, createDevice(null))).toBeFalse();
    expect(service.getCareStatusLabel(plant, createDevice(null))).toBe('Waiting for sensor reading');
  });

  it('describes sensor-based care status', () => {
    const plant = createPlant(50);

    expect(service.getCareStatusLabel(plant, createDevice(39))).toBe('Moisture low');
    expect(service.getCareStatusLabel(plant, createDevice(50))).toBe('Moisture in range');
    expect(service.getCareStatusLabel(plant, createDevice(61))).toBe('Moisture high');
  });

  function createPlant(target: number): Plant {
    return {
      id: 1,
      name: 'Fern',
      wateringInterval: 7,
      desiredSoilMoisturePercentage: target,
      createdAt: new Date(),
      updatedAt: new Date()
    };
  }

  function createDevice(percentage: number | null): WateringDevice {
    return {
      id: 1,
      active: true,
      deleted: false,
      wateringIntervalSetting: 7,
      wateringDurationSetting: 3,
      plantId: 1,
      activeWateringCommandStatus: null,
      latestSoilMoistureMeasurement: percentage === null ? null : {
        rawSoilMoisture: 2350,
        soilMoisturePercentage: percentage,
        measuredAtUtc: new Date()
      }
    };
  }
});
