import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateSerieDto {
  title?: string;
  genre?: string;
  releaseDate?: string;
  image?: string;
  rating?: string;
  director?: string;
}

export interface SeasonDto extends EntityDto<number> {
  number?: number;
  description?: string;
  releaseDate?: string;
  chapters?: string;
}

export interface SerieDto extends EntityDto<number> {
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
  totalSeasons?: number;
  seasons?: SeasonDto[];
}
