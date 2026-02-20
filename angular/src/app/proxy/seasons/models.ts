import type { EntityDto } from '@abp/ng.core';

export interface SeasonDto extends EntityDto<number> {
  number: number;
  description?: string;
  releaseDate?: string;
  chapters?: string;
}
