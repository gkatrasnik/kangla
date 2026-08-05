import { Injectable, NgZone } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  IRetryPolicy,
  LogLevel,
  RetryContext
} from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ClientStateChange } from './client-state-change';

const reconnectDelays = [0, 2000, 5000, 10000, 30000, 60000];

export function reconnectDelay(attempt: number, randomValue = Math.random()): number {
  const baseDelay = reconnectDelays[Math.min(attempt, reconnectDelays.length - 1)];
  if (baseDelay === 0) {
    return 0;
  }

  return Math.round(baseDelay * (0.8 + randomValue * 0.4));
}

@Injectable({ providedIn: 'root' })
export class RealtimeUpdatesService {
  private readonly changesSubject = new Subject<ClientStateChange>();
  private readonly resyncSubject = new Subject<void>();
  private readonly retryPolicy: IRetryPolicy = {
    nextRetryDelayInMilliseconds: (context: RetryContext) => reconnectDelay(context.previousRetryCount)
  };

  private connection: HubConnection | null = null;
  private initialized = false;
  private shouldBeConnected = false;
  private initialRetryAttempt = 0;
  private initialRetryTimer: ReturnType<typeof setTimeout> | null = null;

  readonly changes$: Observable<ClientStateChange> = this.changesSubject.asObservable();
  readonly resync$: Observable<void> = this.resyncSubject.asObservable();

  constructor(
    private authService: AuthService,
    private zone: NgZone
  ) {}

  initialize(): void {
    if (this.initialized) {
      return;
    }

    this.initialized = true;
    this.authService.onStateChanged().subscribe(authenticated => {
      this.shouldBeConnected = authenticated;
      if (authenticated) {
        this.ensureConnection();
      } else {
        void this.stopConnection();
      }
    });
  }

  private ensureConnection(): void {
    if (!this.connection) {
      this.connection = this.createConnection();
    }

    if (this.connection.state === HubConnectionState.Disconnected && !this.initialRetryTimer) {
      void this.startConnection();
    }
  }

  private createConnection(): HubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl('/hubs/client-updates', {
        accessTokenFactory: () => localStorage.getItem('accessToken') ?? ''
      })
      .withAutomaticReconnect(this.retryPolicy)
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on('ClientStateChanged', (change: ClientStateChange) => {
      this.zone.run(() => this.changesSubject.next(change));
    });
    connection.onreconnected(() => {
      this.initialRetryAttempt = 0;
      this.zone.run(() => this.resyncSubject.next());
    });
    connection.onclose(() => {
      if (this.shouldBeConnected) {
        this.scheduleInitialRetry();
      }
    });

    return connection;
  }

  private async startConnection(): Promise<void> {
    if (!this.shouldBeConnected || !this.connection || this.connection.state !== HubConnectionState.Disconnected) {
      return;
    }

    try {
      const reconnectingAfterInitialFailure = this.initialRetryAttempt > 0;
      await this.connection.start();
      this.initialRetryAttempt = 0;
      if (reconnectingAfterInitialFailure) {
        this.zone.run(() => this.resyncSubject.next());
      }
    } catch {
      this.scheduleInitialRetry();
    }
  }

  private scheduleInitialRetry(): void {
    if (!this.shouldBeConnected || this.initialRetryTimer) {
      return;
    }

    const delay = reconnectDelay(this.initialRetryAttempt);
    this.initialRetryAttempt++;
    this.initialRetryTimer = setTimeout(() => {
      this.initialRetryTimer = null;
      void this.startConnection();
    }, delay);
  }

  private async stopConnection(): Promise<void> {
    if (this.initialRetryTimer) {
      clearTimeout(this.initialRetryTimer);
      this.initialRetryTimer = null;
    }

    this.initialRetryAttempt = 0;
    if (this.connection && this.connection.state !== HubConnectionState.Disconnected) {
      await this.connection.stop();
    }
  }
}
