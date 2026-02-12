import type { EntityDto } from '@abp/ng.core';

export interface NotificationDto extends EntityDto<number> {
  userId?: string;
  description?: string;
  type?: string;
  wasRead: boolean;
  sentTime?: string;
}
