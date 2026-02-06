using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Modularity;
using ScriptedReviews.EntityFrameworkCore;

namespace ScriptedReviews;

[DependsOn(
    typeof(ScriptedReviewsApplicationModule),
    typeof(ScriptedReviewsDomainTestModule)
)]
public class ScriptedReviewsApplicationTestModule : AbpModule
{
}