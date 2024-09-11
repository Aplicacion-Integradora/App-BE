using Volo.Abp.Modularity;

namespace ScriptedReviews;

[DependsOn(
    typeof(ScriptedReviewsApplicationModule),
    typeof(ScriptedReviewsDomainTestModule)
)]
public class ScriptedReviewsApplicationTestModule : AbpModule
{

}
