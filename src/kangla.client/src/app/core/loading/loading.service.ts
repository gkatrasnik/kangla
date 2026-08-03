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
  private activeRequests = 0;

  loadingOn(text?: string) {
    this.activeRequests++;
    if (text) {
      this.loadingTextSubject.next(text);
    }
    this.loadingSubject.next(true);
  }

  loadingOff() {
    this.activeRequests = Math.max(0, this.activeRequests - 1);
    if (this.activeRequests === 0) {
      this.loadingSubject.next(false);
      this.resetLoadingText();
    }
  }

  private resetLoadingText() {
    this.loadingTextSubject.next('Loading...');
  }
}
