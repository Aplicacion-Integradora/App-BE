using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using ScriptedReviews.Series;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Emailing;

namespace ScriptedReviews;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
)]
public class ScriptedReviewsTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();
        context.Services.AddSingleton<IEmailSender, NullEmailSender>();

        // Register custom message handler for mocking HttpClient
        context.Services.AddSingleton<FakeHttpMessageHandler>();

        context.Services.AddHttpClient(string.Empty)
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<FakeHttpMessageHandler>());

        var seriesApiMock = Substitute.For<ISeriesApiService>();

        context.Services.Replace(ServiceDescriptor.Singleton<ISeriesApiService>(seriesApiMock));
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
