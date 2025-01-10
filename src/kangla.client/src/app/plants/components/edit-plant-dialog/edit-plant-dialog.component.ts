import { Component, Inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators  } from '@angular/forms';
import { Plant } from '../../plant';
import { ImagesService } from '../../../shared/services/images.service';
import { ImageSrcDirective } from '../../../core/directives/imagesrc.directive';

@Component({
  selector: 'app-plant-device-dialog',
  standalone: true,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    ImageSrcDirective
  ],
  templateUrl: './edit-plant-dialog.component.html',
  styleUrl: './edit-plant-dialog.component.scss'
})
export class EditPlantDialogComponent {
  plantForm: FormGroup;
  private deviceId: number;
  originalPlant: Plant;
  formChanged: boolean = false;
  selectedFile: File | null = null;
  imageUrl: string | undefined;
  imageRemoved: boolean = false;

  constructor(
    private formBuilder: FormBuilder, 
    private dialogRef: MatDialogRef<EditPlantDialogComponent>,
    private imagesService: ImagesService, 
    @Inject (MAT_DIALOG_DATA) public data: Plant) 
    {
      this.deviceId = data.id;
      this.originalPlant = { ...data }
      this.imageUrl = this.imagesService.getImageUrl(data.imageId);
      this.plantForm = this.formBuilder.group({
        name: [data.name, [Validators.required, Validators.maxLength(50)]],
        scientificName: [data.scientificName || '', [Validators.maxLength(100)]],
        description: [data.description || '', [Validators.maxLength(500)]],
        location: [data.location || '', [Validators.maxLength(100)]],
        notes: [data.notes || '', [Validators.maxLength(500)]],
        wateringInterval: [data.wateringInterval, [Validators.required, Validators.min(1), Validators.max(365)]],
        wateringInstructions: [data.wateringInstructions || '', [Validators.maxLength(500)]],
      });

      this.plantForm.valueChanges.subscribe(() => {
        this.formChanged = !this.formsAreIdentical(this.originalPlant, this.plantForm.value) || this.selectedFile !== null;
      });
  }

  async onSubmit(): Promise<void> {
    if (this.plantForm.valid) {
      const formValues = this.plantForm.value;
      const formData = new FormData();
  
      formData.append('name', formValues.name);
      formData.append('scientificName', formValues.scientificName || '');
      formData.append('description', formValues.description || '');
      formData.append('location', formValues.location || '');
      formData.append('notes', formValues.notes || '');
      formData.append('wateringInterval', formValues.wateringInterval);
      formData.append('wateringInstructions', formValues.wateringInstructions || '');
  
      if (this.selectedFile) {
        try {
          const resizedFile = await this.imagesService.resizeImage(this.selectedFile, 512, 512);
          formData.append('image', resizedFile, resizedFile.name);
        } catch (error) {
          console.error("Error resizing image:", error);
          throw new Error('Error uploading image.');
        }
      } else if (this.imageRemoved) {
        formData.append('removeImage', 'true');
      }
  
      this.dialogRef.close(formData);
    }    
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.formChanged = true;
    }
  }

  onRemoveImage(): void {
    this.selectedFile = null;
    this.imageRemoved = true;
    this.formChanged = true;
    this.imageUrl = "";
  }

  private formsAreIdentical(original: Plant, current: any): boolean {
    return (
      original.name === current.name &&
      original.scientificName === current.scientificName &&
      original.description === current.description &&
      original.location === current.location &&
      original.notes === current.notes &&
      original.wateringInterval === current.wateringInterval &&
      original.wateringInstructions === current.wateringInstructions
    );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
