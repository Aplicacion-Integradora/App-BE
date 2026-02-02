using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace ScriptedReviews;

[DependsOn(
    typeof(ScriptedReviewsApplicationModule),
    typeof(ScriptedReviewsDomainTestModule),
    typeof(AbpTestBaseModule)
)]
public class ScriptedReviewsApplicationTestModule : AbpModule
{

}
