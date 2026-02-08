using ScriptedReviews.Series;
using ScriptedReviews.Watchlists.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Watchlists
{
    public interface IWatchlistAppService : IApplicationService
    {
        Task<List<SerieDto>> GetMyWatchlistAsync();
        Task AddSerieAsync(int serieId);
        Task RemoveSerieAsync(int serieId);
        Task<List<WatchlistDto>> GetSeriesWithChangesAsync();
        Task ClearChangesAsync();
    }
}
