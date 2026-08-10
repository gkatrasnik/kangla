import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type PwaInstallPromptType = 'native' | 'ios';

interface BeforeInstallPromptEvent extends Event {
  prompt(): Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed'; platform: string }>;
}

@Injectable({ providedIn: 'root' })
export class PwaInstallPromptService {
  private static readonly deferredUntilKey = 'kangla.pwa-install-prompt.deferred-until';
  private static readonly neverShowKey = 'kangla.pwa-install-prompt.never-show';
  private static readonly deferDurationMs = 7 * 24 * 60 * 60 * 1000;

  private readonly window: Window | null;
  private deferredInstallPrompt: BeforeInstallPromptEvent | null = null;
  private readonly promptSubject = new BehaviorSubject<PwaInstallPromptType | null>(null);

  readonly prompt$ = this.promptSubject.asObservable();

  constructor(@Inject(DOCUMENT) document: Document) {
    this.window = document.defaultView;

    if (!this.window) {
      return;
    }

    this.window.addEventListener('beforeinstallprompt', this.onBeforeInstallPrompt);
    this.window.addEventListener('appinstalled', this.onAppInstalled);
    this.refreshPrompt();
  }

  async install(): Promise<void> {
    if (!this.deferredInstallPrompt) {
      return;
    }

    const installPrompt = this.deferredInstallPrompt;
    this.deferredInstallPrompt = null;
    await installPrompt.prompt();

    const { outcome } = await installPrompt.userChoice;
    if (outcome === 'dismissed') {
      this.defer();
      return;
    }

    this.refreshPrompt();
  }

  defer(): void {
    this.window?.localStorage.setItem(
      PwaInstallPromptService.deferredUntilKey,
      String(Date.now() + PwaInstallPromptService.deferDurationMs)
    );
    this.promptSubject.next(null);
  }

  declinePermanently(): void {
    this.window?.localStorage.setItem(PwaInstallPromptService.neverShowKey, 'true');
    this.promptSubject.next(null);
  }

  private readonly onBeforeInstallPrompt = (event: Event): void => {
    event.preventDefault();
    this.deferredInstallPrompt = event as BeforeInstallPromptEvent;
    this.refreshPrompt();
  };

  private readonly onAppInstalled = (): void => {
    this.deferredInstallPrompt = null;
    this.clearPreferences();
    this.promptSubject.next(null);
  };

  private refreshPrompt(): void {
    if (!this.window || this.isInstalled() || this.isSuppressed()) {
      this.promptSubject.next(null);
      return;
    }

    if (this.deferredInstallPrompt) {
      this.promptSubject.next('native');
      return;
    }

    this.promptSubject.next(this.isIosDevice() ? 'ios' : null);
  }

  private isInstalled(): boolean {
    return this.window?.matchMedia('(display-mode: standalone)').matches === true
      || (this.window?.navigator as Navigator & { standalone?: boolean }).standalone === true;
  }

  private isSuppressed(): boolean {
    if (!this.window) {
      return true;
    }

    if (this.window.localStorage.getItem(PwaInstallPromptService.neverShowKey) === 'true') {
      return true;
    }

    const deferredUntil = Number(this.window.localStorage.getItem(PwaInstallPromptService.deferredUntilKey));
    return Number.isFinite(deferredUntil) && deferredUntil > Date.now();
  }

  private isIosDevice(): boolean {
    if (!this.window) {
      return false;
    }

    const { navigator } = this.window;
    return /iPad|iPhone|iPod/.test(navigator.userAgent)
      || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
  }

  private clearPreferences(): void {
    this.window?.localStorage.removeItem(PwaInstallPromptService.deferredUntilKey);
    this.window?.localStorage.removeItem(PwaInstallPromptService.neverShowKey);
  }
}
