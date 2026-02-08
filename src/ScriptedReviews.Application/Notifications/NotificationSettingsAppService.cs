using ScriptedReviews.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace ScriptedReviews.Notifications
{
    public class NotificationSettingsAppService : ApplicationService
    {
        private readonly ISettingProvider _settingProvider;
        private readonly ISettingManager _settingManager;

        public NotificationSettingsAppService(
           ISettingProvider settingProvider,
           ISettingManager settingManager)
        {
            _settingProvider = settingProvider;
            _settingManager = settingManager;
        }

        public async Task<bool> GetEmailEnabledAsync()
        {
            return await _settingProvider.GetAsync<bool>(
                Settings.NotificationSettings.EmailEnabled
            );
        }

        public async Task SetEmailEnabledAsync(bool enabled)
        {
            await _settingManager.SetForUserAsync(
                CurrentUser.GetId(),
                NotificationSettings.EmailEnabled,
                enabled.ToString()
            );
        }
    }
}
