import type { WatchlistDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class WatchlistService {
  apiName = 'Default';
  

  addSerie = (serieId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/watchlist/serie/${serieId}`,
    },
    { apiName: this.apiName,...config });
  

  getSeriesWithChanges = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, WatchlistDto[]>({
      method: 'GET',
      url: '/api/app/watchlist/series-with-changes',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
