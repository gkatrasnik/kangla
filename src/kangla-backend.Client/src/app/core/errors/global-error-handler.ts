import { ErrorHandler, Injectable } from '@angular/core';
import { ErrorHandlerService } from './error-handler.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor (private errorHandlerService: ErrorHandlerService) {}

  handleError(error: Error | HttpErrorResponse): void {
    this.errorHandlerService.handleError(error);
  }
}