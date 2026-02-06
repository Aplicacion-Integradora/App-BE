using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using ScriptedReviews.MonitoringLogs;

namespace ScriptedReviews.MonitoringLogs;

public class ApiMonitoringAppService : ApplicationService, IApiMonitoringAppService
{
    private readonly IRepository<ApiMonitoringLog, int> _apiMonitoringRepository;

    public ApiMonitoringAppService(IRepository<ApiMonitoringLog, int> apiMonitoringRepository)
    {
        _apiMonitoringRepository = apiMonitoringRepository;
    }

    public async Task<List<ApiMonitoringLogDto>> GetApiLogsAsync()
    {
        var logs = await _apiMonitoringRepository.GetListAsync();
        return ObjectMapper.Map<List<ApiMonitoringLog>, List<ApiMonitoringLogDto>>(logs);
    }
}
