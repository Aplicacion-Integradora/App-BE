import type { EntityDto } from '@abp/ng.core';
import type { SerieDto } from '../../series/models';

export interface WatchlistDto extends EntityDto<number> {
  series: SerieDto[];
  id: number;
  name?: string;
  hasChanges: boolean;
}
