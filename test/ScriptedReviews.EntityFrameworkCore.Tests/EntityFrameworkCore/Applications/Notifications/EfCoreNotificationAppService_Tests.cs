using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Notifications;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreNotificationAppService_Tests : NotificationAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
