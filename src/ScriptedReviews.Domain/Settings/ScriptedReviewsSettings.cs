using Volo.Abp.Settings;

namespace ScriptedReviews.Settings
{
    public static class ScriptedReviewsSettings
    {
        private const string Prefix = "ScriptedReviews";

        //Add your own setting names here. Example:
        //public const string MySetting1 = Prefix + ".MySetting1";
    }

    public class NotificationSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition(
                    NotificationSettings.EmailEnabled,
                    defaultValue: "true",
                    isVisibleToClients: true
                )
            );
        }
    }

}