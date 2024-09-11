using Microsoft.Extensions.Localization;
using ScriptedReviews.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ScriptedReviews;

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
