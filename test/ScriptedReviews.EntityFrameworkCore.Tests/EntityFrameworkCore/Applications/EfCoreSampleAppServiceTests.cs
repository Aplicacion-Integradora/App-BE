using ScriptedReviews.Samples;
using Xunit;

namespace ScriptedReviews.EntityFrameworkCore.Applications;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
