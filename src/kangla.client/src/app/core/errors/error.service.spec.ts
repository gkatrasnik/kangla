import { TestBed } from '@angular/core/testing';

import { ErrorService } from './error.service';

describe('ErrorService', () => {
  let service: ErrorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ErrorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('parses a problem details response returned as text', () => {
    const result = service.parseErrorResponse({
      status: 401,
      error: JSON.stringify({ title: 'Unauthorized', status: 401, detail: 'Failed' })
    });

    expect(result).toEqual({
      title: 'Unauthorized',
      errors: ['Failed'],
      statusCode: 401
    });
  });

  it('parses a problem details response returned as an object', () => {
    const result = service.parseErrorResponse({
      status: 400,
      error: {
        title: 'Validation failed',
        status: 400,
        errors: { email: ['Email is invalid'] }
      }
    });

    expect(result).toEqual({
      title: 'Validation failed',
      errors: ['Email is invalid'],
      statusCode: 400
    });
  });

  it('returns a connection message for a network failure', () => {
    const result = service.parseErrorResponse({ status: 0, error: {} });

    expect(result).toEqual({
      title: 'An error occurred',
      errors: ['Check your connection and try reloading the application.'],
      statusCode: 0
    });
  });
});
