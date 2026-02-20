using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.ErrorLogs;

public interface IErrorLogAppService : IApplicationService
{
    Task<List<ErrorLogDto>> GetErrorLogsAsync();
    
    Task<List<ErrorLogDto>> GetErrorLogsByLevelAsync(string level);
}
