using ScriptedReviews.Series;
using ScriptedReviews.Watchlists.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Watchlist 
{
    public interface IWatchListAppService : IApplicationService
    {
        Task<List<SerieDto>> GetMyWatchlistAsync();
        Task AddSerieAsync(int serieId);
        Task RemoveSerieAsync(int serieId);
        Task<List<WatchlistDto>> GetSeriesWithChangesAsync();
    }
}