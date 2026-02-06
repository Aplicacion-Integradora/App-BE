using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace ScriptedReviews;

[DependsOn(
    typeof(ScriptedReviewsApplicationModule),
    typeof(ScriptedReviewsDomainTestModule)

)]
public class ScriptedReviewsApplicationTestModule : AbpModule
{

}
