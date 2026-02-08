using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using ScriptedReviews.Settings;

namespace ScriptedReviews.Notifications
{
    public class NotificationGenerator : DomainService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IWatchlistAppService _watchlistAppService;
        private readonly IEmailNotificationSender _emailSender;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ISettingProvider _settingProvider;



        public NotificationGenerator(
            IRepository<Notification, int> notificationRepository,
            IWatchlistAppService watchlistAppService,
            IIdentityUserRepository userRepository,
            ISettingProvider settingProvider,
            IEmailNotificationSender emailSender)
        {
            _notificationRepository = notificationRepository;
            _watchlistAppService = watchlistAppService;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _settingProvider = settingProvider;
        }

        public async Task GenerateAsync()
        {
            var seriesWithChanges = await _watchlistAppService.GetSeriesWithChangesAsync();

            foreach (var series in seriesWithChanges)
            {
                var notification = new Notification
                {
                    UserId = series.UserId,
                    Description = $"La serie '{series.Name}' tuvo cambios recientes.",
                    WasRead = false,
                    SentTime = Clock.Now
                };

                await _notificationRepository.InsertAsync(notification);

                var user = await _userRepository.GetAsync(series.UserId);

                // Check if email notifications are enabled for this user
                var emailEnabled = await _settingProvider.GetAsync<bool>(NotificationSettings.EmailEnabled);

                if (emailEnabled && !string.IsNullOrWhiteSpace(user.Email))
                {
                    await _emailSender.SendSeriesUpdateAsync(
                        user.Email,
                        series.Name
                    );
                }
            }
        }

    }
}
