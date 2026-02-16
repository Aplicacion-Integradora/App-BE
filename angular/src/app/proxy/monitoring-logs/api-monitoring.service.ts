import type { ApiMonitoringLogDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiMonitoringService {
  apiName = 'Default';
  

  getApiLogs = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMonitoringLogDto[]>({
      method: 'GET',
      url: '/api/app/api-monitoring/api-logs',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
