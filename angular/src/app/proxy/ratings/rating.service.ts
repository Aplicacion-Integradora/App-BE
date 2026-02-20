import type { CreateRatingDto, RatingDto, UpdateRatingDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RatingService {
  apiName = 'Default';
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto[]>({
      method: 'GET',
      url: '/api/app/rating',
    },
    { apiName: this.apiName,...config });
  

  rate = (input: CreateRatingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto>({
      method: 'POST',
      url: '/api/app/rating/rate',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateRating = (seriesId: number, input: UpdateRatingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto>({
      method: 'PUT',
      url: `/api/app/rating/rating/${seriesId}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
