import { HttpClient, HttpErrorResponse, HttpResponse, HttpStatusCode } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserInfoDto } from "../../auth/user-info-dto";
import {
  BehaviorSubject,
  Observable,
  catchError,
  distinctUntilChanged,
  finalize,
  map,
  of,
  shareReplay,
  switchMap,
  tap,
  throwError
} from "rxjs";
import { environment } from '../../../environments/environment';

type AuthenticationState = 'unknown' | 'authenticated' | 'anonymous';

interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(private http: HttpClient) { }

  private apiUrl = environment.apiUrl;
  private readonly authState = new BehaviorSubject<AuthenticationState>('unknown');
  private readonly userInfo = new BehaviorSubject<UserInfoDto | null>(null);
  private sessionCheck$: Observable<boolean> | null = null;
  private refreshRequest$: Observable<TokenResponse> | null = null;

  public onStateChanged(): Observable<boolean> {
    return this.authState.pipe(
      map((state) => state === 'authenticated'),
      distinctUntilChanged()
    );
  }

  public get userInfo$(): Observable<UserInfoDto | null> {
    return this.userInfo.asObservable();
  }

  //To login with cookies: /login?useCookies=true
  //At the moment we use Bearer token authentication
  public login(email: string, password: string): Observable<boolean> {
    return this.http.post(`${this.apiUrl}/login`, {
      email: email,
      password: password
    }, {
      observe: 'response',
      responseType: 'text'
    }).pipe(
      switchMap((res: HttpResponse<string>) => {
        if (!res.body) {
          this.clearAuthentication();
          return of(false);
        }

        const responseBody = JSON.parse(res.body) as TokenResponse;
        this.storeTokens(responseBody);
        this.authState.next('authenticated');

        return this.fetchUserInfo().pipe(
          map(() => res.ok),
          // Login succeeded even if profile loading temporarily fails.
          catchError(() => of(res.ok))
        );
      })
    );
  }

  public register(email: string, password: string) {
    return this.http.post(`${this.apiUrl}/register`, {
      email: email,
      password: password
    }, {
      observe: 'response',
      responseType: 'text'
    })
      .pipe(
        map((res: HttpResponse<string>) => {
        return res.ok;
      })
    );
  }

  public logout(): Observable<boolean> {
    return this.http.post(`${this.apiUrl}/logout`, {}, {
      withCredentials: true,
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map(() => true), // Logout succeeded
      catchError((error) => {
        console.error("Logout API failed:", error);
        return of(true); // return `true` even if the API fails
      }),
      finalize(() => this.clearAuthentication())
    );
  }

  private fetchUserInfo(): Observable<UserInfoDto> {
    return this.http.get<UserInfoDto>(`${this.apiUrl}/manage/info`).pipe(
      tap((userInfo) => this.userInfo.next(userInfo))
    );
  }

  public ensureAuthenticated(): Observable<boolean> {
    if (this.authState.value === 'authenticated') {
      return of(true);
    }

    if (this.authState.value === 'anonymous') {
      return of(false);
    }

    if (this.sessionCheck$) {
      return this.sessionCheck$;
    }

    const accessToken = localStorage.getItem('accessToken');
    const refreshToken = localStorage.getItem('refreshToken');

    if (!accessToken && !refreshToken) {
      this.clearAuthentication();
      return of(false);
    }

    const prepareAccessToken$ = accessToken
      ? of(null)
      : this.refreshAccessToken().pipe(map(() => null));

    this.sessionCheck$ = prepareAccessToken$.pipe(
      switchMap(() => this.fetchUserInfo()),
      map((userInfo) => {
        const valid = !!(userInfo && userInfo.email && userInfo.email.length > 0);
        this.authState.next(valid ? 'authenticated' : 'anonymous');
        return valid;
      }),
      catchError(() => {
        if (this.authState.value === 'anonymous' || !localStorage.getItem('accessToken')) {
          return of(false);
        }

        // A transient error must not turn an existing local session into a logout.
        // Protected API endpoints remain responsible for enforcing authentication.
        this.authState.next('authenticated');
        return of(true);
      }),
      finalize(() => this.sessionCheck$ = null),
      shareReplay({ bufferSize: 1, refCount: false })
    );

    return this.sessionCheck$;
  }

  public refreshAccessToken(): Observable<TokenResponse> {
    if (this.refreshRequest$) {
      return this.refreshRequest$;
    }

    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      this.clearAuthentication();
      return throwError(() => new Error('No refresh token found'));
    }

    this.refreshRequest$ = this.http.post<TokenResponse>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap((response) => {
        this.storeTokens(response);
        this.authState.next('authenticated');
      }),
      catchError((error: unknown) => {
        if (this.isRejectedCredential(error)) {
          this.clearAuthentication();
        }

        return throwError(() => error);
      }),
      finalize(() => this.refreshRequest$ = null),
      shareReplay({ bufferSize: 1, refCount: false })
    );

    return this.refreshRequest$;
  }

  public clearAuthentication(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.userInfo.next(null);
    this.authState.next('anonymous');
    this.sessionCheck$ = null;
  }

  public hasStoredCredentials(): boolean {
    return !!(localStorage.getItem('accessToken') || localStorage.getItem('refreshToken'));
  }

  private storeTokens(response: TokenResponse): void {
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
  }

  private isRejectedCredential(error: unknown): boolean {
    return error instanceof HttpErrorResponse
      && (error.status === HttpStatusCode.BadRequest
        || error.status === HttpStatusCode.Unauthorized
        || error.status === HttpStatusCode.Forbidden);
  }

  public resendConfirmationEmail(email: string): Observable<boolean> {
    return this.http.post(`${this.apiUrl}/resendConfirmationEmail`, { email }, {
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map((res: HttpResponse<string>) => {
        return res.ok;
      })
    );
  }

  public forgotPassword(email: string): Observable<boolean> {
    return this.http.post(`${this.apiUrl}/forgotPassword`, { email }, {
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map((res: HttpResponse<string>) => {
        return res.ok;
      })
    );
  }

  public resetPassword(email: string, resetCode: string, newPassword: string): Observable<boolean> {
    return this.http.post(`${this.apiUrl}/resetPassword`, {
      email: email,
      resetCode: resetCode,
      newPassword: newPassword
    }, {
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map((res: HttpResponse<string>) => {
        return res.ok;
      })      
    );
  }
}
