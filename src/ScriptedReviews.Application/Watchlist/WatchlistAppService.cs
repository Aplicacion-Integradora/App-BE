using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Watchlist
{
    public class WatchlistAppService : ApplicationService, iWatchlistAppService
    {
        public async Task AddSerieAsync(int serieId)
        {
            throw new NotImplementedException();
        }
    }
}
