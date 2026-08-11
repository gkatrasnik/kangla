import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPlantDialogComponent } from './add-plant-dialog.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImagesService } from '../../../shared/services/images.service';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

describe('AddDeviceDialogComponent', () => {
  let component: AddPlantDialogComponent;
  let fixture: ComponentFixture<AddPlantDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPlantDialogComponent],
      providers: [
        provideNoopAnimations(),
        { provide: MatDialogRef, useValue: { close: jasmine.createSpy('close') } },
        { provide: MAT_DIALOG_DATA, useValue: { desiredSoilMoisturePercentage: 55 } },
        { provide: ImagesService, useValue: { getImageUrl: () => undefined } }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddPlantDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('prefills the recognized moisture target and requires a valid percentage', () => {
    const control = component.plantForm.controls['desiredSoilMoisturePercentage'];

    expect(control.value).toBe(55);
    control.setValue(101);
    expect(control.invalid).toBeTrue();
    control.setValue('');
    expect(control.invalid).toBeTrue();
  });
});
