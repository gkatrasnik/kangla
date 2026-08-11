import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SoilMoistureGaugeComponent } from './soil-moisture-gauge.component';

describe('SoilMoistureGaugeComponent', () => {
  let component: SoilMoistureGaugeComponent;
  let fixture: ComponentFixture<SoilMoistureGaugeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [SoilMoistureGaugeComponent] }).compileComponents();
    fixture = TestBed.createComponent(SoilMoistureGaugeComponent);
    component = fixture.componentInstance;
  });

  it('is green through 10 points, yellow through 20, and red beyond that', () => {
    component.target = 50;
    component.measurement = createMeasurement(50);
    expect(component.gaugeColor).toBe('hsl(120 68% 40%)');

    component.measurement = createMeasurement(40);
    expect(component.gaugeColor).toBe('hsl(120 68% 40%)');

    component.measurement = createMeasurement(30);
    expect(component.gaugeColor).toBe('hsl(48 100% 42%)');

    component.measurement = createMeasurement(29);
    expect(component.gaugeColor).toBe('hsl(0 68% 40%)');
  });

  it('describes whether the reading is above or below the target', () => {
    component.target = 50;
    component.measurement = createMeasurement(42);
    expect(component.statusLabel).toBe('↓ 8%');
    expect(component.statusDescription).toBe('8 points below target');

    component.measurement = createMeasurement(51);
    expect(component.statusLabel).toBe('↑ 1%');
    expect(component.statusDescription).toBe('1 point above target');
  });

  it('renders neutral states for missing readings and targets', () => {
    fixture.detectChanges();
    expect(component.statusLabel).toBe('Waiting for sensor reading');

    component.measurement = createMeasurement(50);
    expect(component.statusLabel).toBe('Set moisture target');
    expect(component.gaugeColor).toBe('var(--app-text-muted)');
  });

  function createMeasurement(soilMoisturePercentage: number) {
    return {
      rawSoilMoisture: 2350,
      soilMoisturePercentage,
      measuredAtUtc: new Date('2026-08-11T10:00:00Z')
    };
  }
});
