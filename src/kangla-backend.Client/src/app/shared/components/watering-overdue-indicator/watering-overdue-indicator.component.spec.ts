import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WateringOverdueIndicatorComponent } from './watering-overdue-indicator.component';

describe('WateringOverdueIndicatorComponent', () => {
  let component: WateringOverdueIndicatorComponent;
  let fixture: ComponentFixture<WateringOverdueIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WateringOverdueIndicatorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WateringOverdueIndicatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
