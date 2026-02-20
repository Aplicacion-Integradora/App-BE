using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Notifications;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreEmailNotificationSender_Tests : EmailNotificationSender_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
