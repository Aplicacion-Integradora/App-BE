using Xunit;

namespace ScriptedReviews.EntityFrameworkCore;

[CollectionDefinition(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class ScriptedReviewsEntityFrameworkCoreCollection : ICollectionFixture<ScriptedReviewsEntityFrameworkCoreFixture>
{

}
