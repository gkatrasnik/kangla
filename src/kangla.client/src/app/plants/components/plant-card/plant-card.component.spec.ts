import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { NotificationService } from '../../../core/notifications/notification.service';
import { DeviceWateringActionService } from '../../../watering-commands/device-watering-action.service';
import { WateringCommand } from '../../../watering-commands/watering-command';
import { PlantWateringActionService } from '../../plant-watering-action.service';
import { PlantService } from '../../plant.service';
import { PlantCardComponent } from './plant-card.component';

describe('PlantCardComponent', () => {
  let component: PlantCardComponent;
  let fixture: ComponentFixture<PlantCardComponent>;
  let send: jasmine.Spy;

  beforeEach(async () => {
    send = jasmine.createSpy('send');
    await TestBed.configureTestingModule({
      imports: [PlantCardComponent],
      providers: [
        provideRouter([]),
        { provide: PlantWateringActionService, useValue: {} },
        { provide: DeviceWateringActionService, useValue: { send } },
        { provide: NotificationService, useValue: {} },
        {
          provide: PlantService,
          useValue: {
            isWateringNeeded: () => false,
            getCareStatusLabel: () => 'Water in 7 days'
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PlantCardComponent);
    component = fixture.componentInstance;
    component.plant = {
      id: 1,
      name: 'Fern',
      wateringInterval: 7,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    component.wateringDevice = {
      id: 10,
      active: true,
      deleted: false,
      wateringIntervalSetting: 7,
      wateringDurationSetting: 3,
      plantId: 1,
      activeWateringCommandStatus: 'pending'
    };
    fixture.detectChanges();
  });

  it('disables device watering while a command is active', () => {
    const button = fixture.nativeElement.querySelector('.plant-card-actions button:last-child') as HTMLButtonElement;

    expect(button.disabled).toBeTrue();
    expect(button.textContent).toContain('Watering queued');
  });

  it('does not render a command badge when the device has no active command', () => {
    component.wateringDevice = { ...component.wateringDevice!, activeWateringCommandStatus: null };

    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('app-watering-command-status-badge')).toBeNull();
  });

  it('emits the created command for immediate parent state updates', () => {
    const command = createCommand();
    component.wateringDevice = { ...component.wateringDevice!, activeWateringCommandStatus: null };
    send.and.returnValue(of(command));
    const emitted: WateringCommand[] = [];
    component.wateringCommandCreated.subscribe(value => emitted.push(value));

    component.sendWateringCommand();

    expect(emitted).toEqual([command]);
  });

  function createCommand(): WateringCommand {
    return {
      id: 20,
      deviceId: 10,
      status: 'pending',
      durationSeconds: 3,
      requestedAtUtc: new Date().toISOString(),
      expiresAtUtc: new Date().toISOString(),
      acknowledgedAtUtc: null,
      startedAtUtc: null,
      finishedAtUtc: null,
      failureReason: null,
      wateringEventId: null
    };
  }
});
