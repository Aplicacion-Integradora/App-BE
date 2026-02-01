using ScriptedReviews.Application.Tests.Ratings;
using ScriptedReviews.Ratings;
using Xunit;

namespace ScriptedReviews.EntityFrameworkCore.Ratings;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreRatingAppService_Tests : RatingAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
