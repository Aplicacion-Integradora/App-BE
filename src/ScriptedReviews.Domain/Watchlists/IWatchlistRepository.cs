using Volo.Abp.Domain.Repositories;

namespace ScriptedReviews.Watchlists
{
    public interface IWatchlistRepository : IRepository<Watchlist, int>
    {
    }
}
