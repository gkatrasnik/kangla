import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WateringEventsTableComponent } from './watering-events-table.component';

describe('WateringEventsTableComponent', () => {
  let component: WateringEventsTableComponent;
  let fixture: ComponentFixture<WateringEventsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WateringEventsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WateringEventsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
