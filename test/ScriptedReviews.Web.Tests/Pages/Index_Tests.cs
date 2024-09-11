using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace ScriptedReviews.Pages;

[Collection(ScriptedReviewsTestConsts.CollectionDefinitionName)]
public class Index_Tests : ScriptedReviewsWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
