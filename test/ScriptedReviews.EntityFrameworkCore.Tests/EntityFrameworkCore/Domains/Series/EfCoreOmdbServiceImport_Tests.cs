using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Series;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class EfCoreOmdbServiceImport_Tests : OmdbServiceImport_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
{

}

