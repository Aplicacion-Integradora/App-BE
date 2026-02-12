
export interface ErrorLogDto {
  id: number;
  level?: string;
  message?: string;
  exception?: string;
  source?: string;
  endpoint?: string;
  httpMethod?: string;
  httpStatusCode?: number;
  userId?: string;
  ipAddress?: string;
  createdAt?: string;
}
