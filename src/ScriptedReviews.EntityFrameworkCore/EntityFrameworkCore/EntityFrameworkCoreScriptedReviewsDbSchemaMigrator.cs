using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScriptedReviews.Data;
using Volo.Abp.DependencyInjection;

namespace ScriptedReviews.EntityFrameworkCore;

public class EntityFrameworkCoreScriptedReviewsDbSchemaMigrator
    : IScriptedReviewsDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreScriptedReviewsDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ScriptedReviewsDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ScriptedReviewsDbContext>()
            .Database
            .MigrateAsync();
    }
}
