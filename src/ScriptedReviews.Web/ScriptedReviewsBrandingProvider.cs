using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using ScriptedReviews.Localization;

namespace ScriptedReviews.Web;

[Dependency(ReplaceServices = true)]
public class ScriptedReviewsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ScriptedReviewsResource> _localizer;

    public ScriptedReviewsBrandingProvider(IStringLocalizer<ScriptedReviewsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
