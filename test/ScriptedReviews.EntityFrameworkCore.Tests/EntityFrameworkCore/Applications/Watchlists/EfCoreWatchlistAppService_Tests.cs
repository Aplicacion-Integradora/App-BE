using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Watchlists;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreWatchlistAppService_Tests : WatchlistAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
