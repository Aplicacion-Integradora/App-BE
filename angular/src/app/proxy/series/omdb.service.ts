import type { SerieDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class OmdbService {
  apiName = 'Default';
  

  getSeries = (title: string, genre: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto[]>({
      method: 'GET',
      url: '/api/app/omdb/series',
      params: { title, genre },
    },
    { apiName: this.apiName,...config });
  

  importarSerie = (titulo: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'POST',
      url: '/api/app/omdb/importar-serie',
      params: { titulo },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
