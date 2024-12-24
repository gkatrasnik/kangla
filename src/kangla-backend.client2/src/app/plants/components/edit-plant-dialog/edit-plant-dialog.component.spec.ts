import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPlantDialogComponent } from './edit-plant-dialog.component';

describe('EditDeviceDialogComponent', () => {
  let component: EditPlantDialogComponent;
  let fixture: ComponentFixture<EditPlantDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPlantDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPlantDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
