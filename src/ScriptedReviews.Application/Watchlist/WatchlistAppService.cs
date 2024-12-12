using ScriptedReviews.Watchlists.Dtos;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Expressions;

namespace ScriptedReviews.Watchlists
{
    public class WatchlistAppService : ApplicationService, iWatchlistAppService
    {
        public async Task AddSerieAsync(int serieId)
        {
            throw new NotImplementedException();
        }

        private readonly IRepository<Watchlist, int> _watchlistRepository;

        public WatchlistAppService(IRepository<Watchlist, int> watchlistRepository)
        {
            _watchlistRepository = watchlistRepository;
        }

        // Método para obtener series con cambios
        public async Task<List<WatchlistDto>> GetSeriesWithChangesAsync()
        {
            var queryableSeries = await _watchlistRepository.GetQueryableAsync();
            var seriesWithChanges = queryableSeries
                .Where(s => s.HasChanges)
                .ToList();

            return ObjectMapper.Map<List<Watchlist>, List<WatchlistDto>>(seriesWithChanges);
            //return ObjectMapper.Map<List<WatchlistDto>>(seriesWithChanges);
        }
    }
}