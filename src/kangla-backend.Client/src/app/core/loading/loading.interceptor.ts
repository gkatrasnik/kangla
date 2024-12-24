import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpContextToken } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { LoadingService } from './loading.service';
import { Observable } from 'rxjs';

export const SkipLoading = new HttpContextToken<boolean>(() => false);

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.context.get(SkipLoading)) {
      return next.handle(req);
    }

    setTimeout(() => {
      this.loadingService.loadingOn();
    }, 0);

    return next.handle(req).pipe(
      finalize(() => {
        setTimeout(() => {
          this.loadingService.loadingOff();
        }, 0);
      })
    );
  }
}
