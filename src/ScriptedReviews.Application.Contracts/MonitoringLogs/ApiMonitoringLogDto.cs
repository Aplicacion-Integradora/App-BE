using System;

namespace ScriptedReviews.MonitoringLogs;

public class ApiMonitoringLogDto
{
    public int Id { get; set; }
    public string Endpoint { get; set; }
    public string HttpMethod { get; set; }
    public long ResponseTime { get; set; }
    public int HttpStatusCode { get; set; }
    public string UserAgent { get; set; }
    public string IPAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
