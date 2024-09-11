using Volo.Abp.Modularity;

namespace ScriptedReviews;

/* Inherit from this class for your domain layer tests. */
public abstract class ScriptedReviewsDomainTestBase<TStartupModule> : ScriptedReviewsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
