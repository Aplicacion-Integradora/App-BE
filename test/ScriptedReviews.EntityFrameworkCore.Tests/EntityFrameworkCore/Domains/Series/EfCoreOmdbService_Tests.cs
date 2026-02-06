using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedReviews.EntityFrameworkCore;
using Xunit;

namespace ScriptedReviews.Series
{
    [Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
    public class EfCoreOmdbService_Tests : OmdbService_Tests<ScriptedReviewsEntityFrameworkCoreTestModule>
    {
    }
}
