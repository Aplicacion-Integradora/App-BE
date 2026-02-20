import type { ErrorLogDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ErrorLogService {
  apiName = 'Default';
  

  getErrorLogs = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ErrorLogDto[]>({
      method: 'GET',
      url: '/api/app/error-log/error-logs',
    },
    { apiName: this.apiName,...config });
  

  getErrorLogsByLevel = (level: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ErrorLogDto[]>({
      method: 'GET',
      url: '/api/app/error-log/error-logs-by-level',
      params: { level },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
