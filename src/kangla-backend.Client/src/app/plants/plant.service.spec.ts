import { TestBed } from '@angular/core/testing';

import { WateringDeviceService } from './plant.service';

describe('WateringDeviceService', () => {
  let service: WateringDeviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WateringDeviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
