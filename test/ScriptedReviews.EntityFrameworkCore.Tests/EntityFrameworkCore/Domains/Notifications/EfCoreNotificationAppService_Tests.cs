using ScriptedReviews.Application.Tests.Notifications;
using Xunit;

namespace ScriptedReviews.EntityFrameworkCore.Applications.Notifications;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreNotificationAppService_Tests : NotificationAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
