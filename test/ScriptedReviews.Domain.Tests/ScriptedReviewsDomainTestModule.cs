using Volo.Abp.Modularity;

namespace ScriptedReviews;

[DependsOn(
    typeof(ScriptedReviewsDomainModule),
    typeof(ScriptedReviewsTestBaseModule)
)]
public class ScriptedReviewsDomainTestModule : AbpModule
{

}
