import { ImageSrcDirective } from './imagesrc.directive';
import { ElementRef } from '@angular/core';

describe('ImageSrcDirective', () => {
  it('should create an instance', () => {
    const mockElementRef = { nativeElement: { src: '' } } as ElementRef<HTMLImageElement>;
    const mockHttpClient = jasmine.createSpyObj('HttpClient', ['get']);

    const directive = new ImageSrcDirective(mockElementRef, mockHttpClient);

    expect(directive).toBeTruthy();
  });
});