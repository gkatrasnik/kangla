import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ImagesService {
  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  resizeImage(file: File, maxWidth: number, maxHeight: number): Promise<File> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = async (event: any) => {
        const img = new Image();
        img.onload = async () => {
          try {
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');
  
            if (!ctx) {
              throw new Error('Could not get canvas context.');
            }
  
            // Calculate the scale to fit the image within the target dimensions
            let width = img.width;
            let height = img.height;
            let scale = Math.max(maxWidth / width, maxHeight / height); // Scale up to fill the target dimensions
  
            let newWidth = width * scale;
            let newHeight = height * scale;
  
            // Calculate cropping offsets
            let offsetX = (newWidth - maxWidth) / 2;
            let offsetY = (newHeight - maxHeight) / 2;
  
            // Set canvas dimensions to target size
            canvas.width = maxWidth;
            canvas.height = maxHeight;
  
            // Draw the image centered and cropped
            ctx.drawImage(img, -offsetX, -offsetY, newWidth, newHeight);
  
            // Convert canvas to Blob and then to File
            const resizedBlob = await new Promise<Blob | null>((resolve, reject) => {
              canvas.toBlob(blob => {
                if (blob) {
                  resolve(blob);
                } else {
                  reject('Blob conversion failed.');
                }
              }, file.type);
            });
  
            if (!resizedBlob) {
              throw new Error('Blob conversion failed.');
            }
  
            const resizedFile = new File([resizedBlob], file.name, { type: file.type });
            resolve(resizedFile);
  
          } catch (error) {
            reject(error);
          }
        };
        img.src = event.target.result;
      };
      reader.onerror = error => reject(error);
      reader.readAsDataURL(file);
    });
  }

  getImageUrl(imageId?: string): string | undefined {
    return imageId ? `${this.apiUrl}/images/${imageId}` : undefined;
  }

  deleteImage(imageId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Images/${imageId}`)
  }
}
