using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ScriptedReviews.ErrorLogs;

public class ErrorLogAppService : ApplicationService, IErrorLogAppService
{
    private readonly IRepository<ErrorLog, int> _errorLogRepository;

    public ErrorLogAppService(IRepository<ErrorLog, int> errorLogRepository)
    {
        _errorLogRepository = errorLogRepository;
    }

    public async Task<List<ErrorLogDto>> GetErrorLogsAsync()
    {
        var logs = await _errorLogRepository.GetListAsync();
        var orderedLogs = logs.OrderByDescending(l => l.CreatedAt).ToList();
        return ObjectMapper.Map<List<ErrorLog>, List<ErrorLogDto>>(orderedLogs);
    }

    public async Task<List<ErrorLogDto>> GetErrorLogsByLevelAsync(string level)
    {
        var query = await _errorLogRepository.GetQueryableAsync();
        var logs = query
            .Where(l => l.Level == level)
            .OrderByDescending(l => l.CreatedAt)
            .ToList();
        return ObjectMapper.Map<List<ErrorLog>, List<ErrorLogDto>>(logs);
    }
}
