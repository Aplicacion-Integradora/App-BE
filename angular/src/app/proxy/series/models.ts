import type { EntityDto } from '@abp/ng.core';
import type { SeasonDto } from '../seasons/models';

export interface CreateUpdateSerieDto {
  title: string;
  genre: string;
  releaseDate?: string;
  duration?: string;
  director?: string;
  writer?: string;
  cast?: string;
  image?: string;
  country?: string;
  rating?: string;
  description?: string;
}

export interface SerieDto extends EntityDto<number> {
  id: number;
  title?: string;
  description?: string;
  image?: string;
  genre?: string;
  language?: string;
  releaseDate?: string;
  duration?: string;
  rating?: string;
  country?: string;
  director?: string;
  cast?: string;
  writer?: string;
  imdbId?: string;
  totalSeasons: number;
  seasons: SeasonDto[];
}
