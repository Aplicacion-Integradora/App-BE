using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Ratings;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreRatingAppService_Tests : RatingAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
