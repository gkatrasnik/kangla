import { Injectable } from '@angular/core';
import { AuthErrorResponse } from './auth-error-response';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  constructor() {}
    
    parseErrorResponse(error: any): { title: string; errors: string[], statusCode: number } {
      let title = 'An error occurred';
      let errors: string[] = [];
      let statusCode = typeof error?.status === 'number' ? error.status : 0;
      let errorResponse: AuthErrorResponse | null = null;

      if (typeof error?.error === 'string') {
        try {
          errorResponse = JSON.parse(error.error) as AuthErrorResponse;
        } catch (e) {
          console.error('Failed to parse error response', e);
        }
      } else if (error?.error && typeof error.error === 'object') {
        errorResponse = error.error as AuthErrorResponse;
      }

      if (errorResponse) {
        title = errorResponse.title || title;
        statusCode = errorResponse.status || statusCode;
        if (errorResponse.errors) {
          errors = Object.values(errorResponse.errors).flat();
        }
        if (errorResponse.detail) {
          errors.unshift(errorResponse.detail);
        }
      }

      if (errors.length === 0) {
        errors.push(statusCode === 0
          ? 'Check your connection and try reloading the application.'
          : 'Please try again.');
      }
  
      return { title, errors, statusCode };
    }
}
