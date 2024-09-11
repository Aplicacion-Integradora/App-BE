using ScriptedReviews.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace ScriptedReviews.Web.Pages;

public abstract class ScriptedReviewsPageModel : AbpPageModel
{
    protected ScriptedReviewsPageModel()
    {
        LocalizationResourceType = typeof(ScriptedReviewsResource);
    }
}
