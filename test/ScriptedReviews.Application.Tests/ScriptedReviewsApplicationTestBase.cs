using Volo.Abp.Modularity;

namespace ScriptedReviews;

public abstract class ScriptedReviewsApplicationTestBase<TStartupModule> : ScriptedReviewsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
