import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WateringCommandStatusBadgeComponent } from './watering-command-status-badge.component';

describe('WateringCommandStatusBadgeComponent', () => {
  let fixture: ComponentFixture<WateringCommandStatusBadgeComponent>;
  let component: WateringCommandStatusBadgeComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WateringCommandStatusBadgeComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(WateringCommandStatusBadgeComponent);
    component = fixture.componentInstance;
  });

  it('renders a queued pending command', () => {
    component.status = 'pending';
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Watering queued');
    expect(fixture.nativeElement.querySelector('mat-icon').textContent).toContain('schedule');
  });

  it('renders an acknowledged command as watering now', () => {
    component.status = 'acknowledged';
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Watering now');
    expect(fixture.nativeElement.querySelector('mat-icon').textContent).toContain('water_drop');
  });
});
