import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root', 
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private loadingTextSubject = new BehaviorSubject<string>('Loading...');

  loading$ = this.loadingSubject.asObservable();
  loadingText$ = this.loadingTextSubject.asObservable();

  loadingOn(text?: string) {
    if (text) {
      this.loadingTextSubject.next(text);
    }
    this.loadingSubject.next(true);
  }

  loadingOff() {
    this.loadingSubject.next(false);
    this.resetLoadingText();
  }

  private resetLoadingText() {
    this.loadingTextSubject.next('Loading...');
  }
}
