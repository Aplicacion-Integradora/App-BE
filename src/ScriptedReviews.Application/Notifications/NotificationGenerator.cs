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

namespace ScriptedReviews.Notifications
{
    public class NotificationGenerator : DomainService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IWatchlistAppService _watchlistAppService;
        private readonly EmailNotificationSender _emailSender;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ISettingManager _settingManager;



        public NotificationGenerator(
            IRepository<Notification, int> notificationRepository,
            IWatchlistAppService watchlistAppService,
            IIdentityUserRepository userRepository,
            ISettingManager settingManager,
            EmailNotificationSender emailSender)
        {
            _notificationRepository = notificationRepository;
            _watchlistAppService = watchlistAppService;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _settingManager = settingManager;

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

                if (!string.IsNullOrWhiteSpace(user.Email))
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
