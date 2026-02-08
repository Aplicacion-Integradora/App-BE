using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Series;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreSerieAppService_Tests : SerieAppService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}
