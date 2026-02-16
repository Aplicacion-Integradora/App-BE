using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ScriptedReviews.MonitoringLogs;

public class ApiMonitoringLog : AggregateRoot<int>
{
    public string Endpoint { get; private set; }
    public string HttpMethod { get; private set; }
    public long ResponseTime { get; private set; }
    public int HttpStatusCode { get; private set; }
    public string UserAgent { get; private set; }
    public string IPAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Constructor vacío requerido por Entity Framework
    private ApiMonitoringLog() { }

    // Constructor con parámetros para asegurar integridad de datos
    public ApiMonitoringLog(string endpoint, string httpMethod, long responseTime, int httpStatusCode, string userAgent, string ipAddress)
    {
        Endpoint = endpoint;
        HttpMethod = httpMethod;
        ResponseTime = responseTime;
        HttpStatusCode = httpStatusCode;
        UserAgent = userAgent;
        IPAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
    }
}
