import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PlantCardComponent } from './plant-card.component';

describe('WateringDeviceCardComponent', () => {
  let component: PlantCardComponent;
  let fixture: ComponentFixture<PlantCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});