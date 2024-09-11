using Volo.Abp.Settings;

namespace ScriptedReviews.Settings;

public class ScriptedReviewsSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ScriptedReviewsSettings.MySetting1));
    }
}
