using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Series;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreOmdbService_Tests : OmdbService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}

