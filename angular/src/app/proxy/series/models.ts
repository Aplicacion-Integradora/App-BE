import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateSerieDto {
  title?: string;
}

export interface SerieDto extends EntityDto<number> {
  title?: string;
  poster?: string;
  genre?: string;
  year?: string;
}
