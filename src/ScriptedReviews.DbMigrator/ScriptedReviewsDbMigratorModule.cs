using ScriptedReviews.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace ScriptedReviews.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ScriptedReviewsEntityFrameworkCoreModule),
    typeof(ScriptedReviewsApplicationContractsModule)
)]
public class ScriptedReviewsDbMigratorModule : AbpModule
{
}
