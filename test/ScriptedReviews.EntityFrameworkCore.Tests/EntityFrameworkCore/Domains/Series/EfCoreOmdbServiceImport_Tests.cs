using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Series
{
    /// <summary>
    /// EF Core implementation of OmdbServiceImport_Tests for database integration testing
    /// </summary>
    [Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
    public class EfCoreOmdbServiceImport_Tests : OmdbServiceImport_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
    {
    }
}
