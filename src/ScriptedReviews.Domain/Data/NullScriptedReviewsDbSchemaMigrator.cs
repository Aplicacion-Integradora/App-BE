using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ScriptedReviews.Data;

/* This is used if database provider does't define
 * IScriptedReviewsDbSchemaMigrator implementation.
 */
public class NullScriptedReviewsDbSchemaMigrator : IScriptedReviewsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
