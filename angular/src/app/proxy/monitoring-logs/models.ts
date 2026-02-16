
export interface ApiMonitoringLogDto {
  id: number;
  endpoint?: string;
  httpMethod?: string;
  responseTime: number;
  httpStatusCode: number;
  userAgent?: string;
  ipAddress?: string;
  createdAt?: string;
}
