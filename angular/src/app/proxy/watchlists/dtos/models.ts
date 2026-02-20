import type { EntityDto } from '@abp/ng.core';
import type { SerieDto } from '../../series/models';

export interface WatchlistDto extends EntityDto<number> {
  series: SerieDto[];
  name?: string;
  userId?: string;
  hasChanges: boolean;
}
