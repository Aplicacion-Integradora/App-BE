using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.ErrorLogs;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreErrorLogAppService_Tests : ErrorLogAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
