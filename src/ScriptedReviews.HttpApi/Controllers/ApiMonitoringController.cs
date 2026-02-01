using Microsoft.AspNetCore.Mvc;
using ScriptedReviews.MonitoringLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace ScriptedReviews.Controllers;

[Route("api/monitoring")]
public class ApiMonitoringController : AbpController
{
    private readonly IApiMonitoringAppService _apiMonitoringAppService;

    public ApiMonitoringController(IApiMonitoringAppService apiMonitoringAppService)
    {
        _apiMonitoringAppService = apiMonitoringAppService;
    }

    [HttpGet]
    public async Task<List<ApiMonitoringLogDto>> GetLogs()
    {
        return await _apiMonitoringAppService.GetApiLogsAsync();
    }
}