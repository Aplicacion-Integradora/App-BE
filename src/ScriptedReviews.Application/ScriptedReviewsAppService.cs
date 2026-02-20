using ScriptedReviews.Localization;
using Volo.Abp.Application.Services;

namespace ScriptedReviews;

/* Inherit your application services from this class.
 */
public abstract class ScriptedReviewsAppService : ApplicationService
{
    protected ScriptedReviewsAppService()
    {
        LocalizationResource = typeof(ScriptedReviewsResource);
    }
}
