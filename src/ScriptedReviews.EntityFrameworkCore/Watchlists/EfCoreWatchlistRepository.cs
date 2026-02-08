using ScriptedReviews.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ScriptedReviews.Watchlists
{
    public class EfCoreWatchlistRepository
        : EfCoreRepository<ScriptedReviewsDbContext, Watchlist, int>,
          IWatchlistRepository
    {
        public EfCoreWatchlistRepository(
            IDbContextProvider<ScriptedReviewsDbContext> dbContextProvider
        ) : base(dbContextProvider)
        {
        }
    }
}
