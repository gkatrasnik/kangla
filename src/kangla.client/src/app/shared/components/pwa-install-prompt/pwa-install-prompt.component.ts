import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PwaInstallPromptService } from '../../../core/pwa/pwa-install-prompt.service';

@Component({
  selector: 'app-pwa-install-prompt',
  standalone: true,
  imports: [AsyncPipe, MatButtonModule, MatIconModule],
  templateUrl: './pwa-install-prompt.component.html',
  styleUrl: './pwa-install-prompt.component.scss'
})
export class PwaInstallPromptComponent {
  readonly prompt$ = this.pwaInstallPromptService.prompt$;

  constructor(private pwaInstallPromptService: PwaInstallPromptService) {}

  install(): void {
    void this.pwaInstallPromptService.install();
  }

  notNow(): void {
    this.pwaInstallPromptService.defer();
  }

  never(): void {
    this.pwaInstallPromptService.declinePermanently();
  }
}
