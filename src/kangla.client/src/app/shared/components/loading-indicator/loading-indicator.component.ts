import { Component, OnInit, Input, ContentChild, TemplateRef } from '@angular/core';
import { Observable } from 'rxjs';
import { LoadingService } from '../../../core/loading/loading.service';
import { Router, NavigationStart, NavigationEnd } from '@angular/router';
import { tap } from 'rxjs/operators';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AsyncPipe } from '@angular/common';


@Component({
    selector: 'app-loading-indicator',
    templateUrl: './loading-indicator.component.html',
    styleUrls: ['./loading-indicator.component.scss'],
    imports: [MatProgressSpinnerModule, AsyncPipe]
})
export class LoadingIndicatorComponent implements OnInit {

  loading$: Observable<boolean>;
  loadingText$: Observable<string>;

  @Input() detectRouteTransitions = false; 
  @ContentChild('loading') customLoadingIndicator: TemplateRef<any> | null = null;

  constructor(private loadingService: LoadingService, private router: Router) {
    this.loading$ = this.loadingService.loading$; 
    this.loadingText$ = this.loadingService.loadingText$;
  }

  ngOnInit() {
    if (this.detectRouteTransitions) {
      this.router.events
        .pipe(
          tap(event => {
            if (event instanceof NavigationStart) {
              this.loadingService.loadingOn();
            } else if (event instanceof NavigationEnd) {
              this.loadingService.loadingOff();
            }
          })
        )
        .subscribe();
    }
  }
}
