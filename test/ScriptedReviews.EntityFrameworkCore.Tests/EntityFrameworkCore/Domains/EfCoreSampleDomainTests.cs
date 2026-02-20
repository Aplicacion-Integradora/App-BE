using ScriptedReviews.Samples;
using Xunit;

namespace ScriptedReviews.EntityFrameworkCore.Domains;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
