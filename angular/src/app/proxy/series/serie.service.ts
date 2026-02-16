import type { CreateUpdateSerieDto, SerieDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SerieService {
  apiName = 'Default';

  // --- 1. Crear ---
  create = (input: CreateUpdateSerieDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'POST',
      url: '/api/app/serie',
      body: input,
    },
      { apiName: this.apiName, ...config });

  // --- 2. Borrar ---
  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/serie/${id}`,
    },
      { apiName: this.apiName, ...config });

  // --- 3. Obtener Uno ---
  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'GET',
      url: `/api/app/serie/${id}`,
    },
      { apiName: this.apiName, ...config });

  // --- 4. Listar Paginado (Operación 2.1 - BD) ---
  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SerieDto>>({
      method: 'GET',
      url: '/api/app/serie',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });

  // --- 5. Buscar en OMDB (Operación 1.1 - API) ---

  search = (title: string, genre: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto[]>({
      method: 'GET',
      url: '/api/app/serie/search',
      params: { title, genre },
    },
      { apiName: this.apiName, ...config });

  // --- 6. Actualizar ---
  update = (id: number, input: CreateUpdateSerieDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'PUT',
      url: `/api/app/serie/${id}`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  // --- 7. IMPORTAR SERIE (Operación 2.2) ---
  importarSerie = (imdbId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'POST',
      url: '/api/app/serie/importar-serie',
      params: { imdbId },
    },
      { apiName: this.apiName, ...config });

  constructor(private restService: RestService) { }
}

