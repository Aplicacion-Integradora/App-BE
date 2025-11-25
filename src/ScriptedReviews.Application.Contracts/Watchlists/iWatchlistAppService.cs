using ScriptedReviews.Watchlists.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Watchlists
{
    public interface IWatchlistAppService : IApplicationService
    {
        Task AddSerieAsync(int serieId);
        Task<List<WatchlistDto>> GetSeriesWithChangesAsync();
    }
}
