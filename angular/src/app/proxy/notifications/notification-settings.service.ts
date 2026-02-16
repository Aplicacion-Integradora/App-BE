import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NotificationSettingsService {
  apiName = 'Default';
  

  getEmailEnabled = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'GET',
      url: '/api/app/notification-settings/email-enabled',
    },
    { apiName: this.apiName,...config });
  

  setEmailEnabled = (enabled: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/notification-settings/set-email-enabled',
      params: { enabled },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
