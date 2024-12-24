import { TestBed } from '@angular/core/testing';

import { WateringEventService } from './watering-event.service';

describe('WateringEventService', () => {
  let service: WateringEventService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WateringEventService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
