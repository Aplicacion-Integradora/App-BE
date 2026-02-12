import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { ApiMonitoringLogDto } from '../monitoring-logs/models';

@Injectable({
  providedIn: 'root',
})
export class ApiMonitoringService {
  apiName = 'Default';
  

  getLogs = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMonitoringLogDto[]>({
      method: 'GET',
      url: '/api/monitoring',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
