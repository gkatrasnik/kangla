import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPlantDialogComponent } from './edit-plant-dialog.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImagesService } from '../../../shared/services/images.service';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

describe('EditDeviceDialogComponent', () => {
  let component: EditPlantDialogComponent;
  let fixture: ComponentFixture<EditPlantDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPlantDialogComponent],
      providers: [
        provideNoopAnimations(),
        { provide: MatDialogRef, useValue: { close: jasmine.createSpy('close') } },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            id: 1,
            name: 'Fern',
            wateringInterval: 7,
            desiredSoilMoisturePercentage: 50,
            createdAt: new Date(),
            updatedAt: new Date()
          }
        },
        { provide: ImagesService, useValue: { getImageUrl: () => undefined } }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPlantDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('requires a desired soil moisture percentage', () => {
    const control = component.plantForm.controls['desiredSoilMoisturePercentage'];

    expect(control.value).toBe(50);
    control.setValue(null);
    expect(control.invalid).toBeTrue();
  });
});
