using ScriptedReviews.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ScriptedReviews.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ScriptedReviewsController : AbpControllerBase
{
    protected ScriptedReviewsController()
    {
        LocalizationResource = typeof(ScriptedReviewsResource);
    }
}
