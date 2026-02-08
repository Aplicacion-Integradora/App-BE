using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.MonitoringLogs;

public interface IApiMonitoringAppService : IApplicationService
{
    Task<List<ApiMonitoringLogDto>> GetApiLogsAsync();
}

