import { Component, Inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators  } from '@angular/forms';
import { ImagesService } from '../../../shared/services/images.service';
import { PlantRecognizeResponseDto } from '../../dto/plant-recognize-response-dto';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';

@Component({
    selector: 'app-add-device-dialog',
    imports: [
        MatDialogModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatInputModule,
        MatButtonModule,
        ImageSrcDirective
    ],
    templateUrl: './add-plant-dialog.component.html',
    styleUrl: './add-plant-dialog.component.scss'
})
export class AddPlantDialogComponent {
  plantForm: FormGroup;
  selectedFile: File | null = null;
  imageId: string | null = null;
  imageUrl: string | undefined;
  
  constructor(
    private formBuilder: FormBuilder, 
    private dialogRef: MatDialogRef<AddPlantDialogComponent>,
    private imagesService: ImagesService,
    @Inject(MAT_DIALOG_DATA) public data: PlantRecognizeResponseDto
  ) {
    this.plantForm = this.formBuilder.group({
      name: [data?.commonName || '', [Validators.required, Validators.maxLength(50)]],
      scientificName: [data?.latinName || '', [Validators.maxLength(100)]],
      description: [data?.description || '', [Validators.maxLength(500)]],
      location: ['', [Validators.maxLength(100)]],
      notes: [data?.additionalTips || '', [Validators.maxLength(500)]],
      wateringInterval: [data?.wateringInterval || '', [Validators.required, Validators.min(1), Validators.max(365)]],
      wateringInstructions: [data?.wateringInstructions || '', [Validators.maxLength(500)]],
      imageId: [data?.imageId || null]
    });

    this.imageId = data?.imageId?.toString() || null;
    this.imageUrl = this.imagesService.getImageUrl(data?.imageId);
  }

  async onSubmit(): Promise<void> {
    if (this.plantForm.valid) {
      // Extract form values
      const formValues = this.plantForm.value;
  
      // Construct the DTO as an object
      const plantCreateDto = {
        name: formValues.name,
        scientificName: formValues.scientificName || '',
        description: formValues.description || '',
        location: formValues.location || '',
        notes: formValues.notes || '',
        wateringInterval: formValues.wateringInterval,
        wateringInstructions: formValues.wateringInstructions || '',
        imageId: this.imageId ? this.imageId : null // Ensure imageId is numeric if present
      };
  
      // Close the dialog and pass the DTO back
      this.dialogRef.close(plantCreateDto);
    }
  }

  onCancel(): void {
    if (this.imageId) {
      //Image was saved when plant was recognized. If we dont want to add a plant, we want to delete the image
      this.imagesService.deleteImage(this.imageId).subscribe(() => {
        this.dialogRef.close();
      });
    } else {
      this.dialogRef.close();
    }        
  }  
}
