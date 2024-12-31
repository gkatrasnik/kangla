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
      let statusCode: number = 0;
  
      if (error && error.error && typeof error.error == 'string') { //api will normally return string
        try {
          const errorResponse: AuthErrorResponse = JSON.parse(error.error);
          title = errorResponse.title || title;
          statusCode = errorResponse.status || 0;
          if (errorResponse.errors) {
            errors = Object.values(errorResponse.errors).flat();
          }
          if (errorResponse.detail) {
            errors.unshift(errorResponse.detail);
          }
        } catch (e) {
          console.error('Failed to parse error response', e);
        }
      } else { //if app is offline, error.error is object
        errors.push("Check your connection and try reloading the application.");
      }
  
      return { title, errors, statusCode };
    }
}
