using ScriptedReviews.EntityFrameworkCore;
using ScriptedReviews.Samples;
using Xunit;

namespace ScriptedReviews.MonitoringLogs;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreApiMonitoringAppServiceTests : ApiMonitoringAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}