using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using ScriptedReviews.Watchlists;

namespace ScriptedReviews.Notifications
{
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<Watchlist, int> _watchlistRepository;
        private readonly IWatchlistAppService _watchlistAppService;

        public NotificationAppService(
            IRepository<Notification, int> notificationRepository,
            IRepository<Watchlist, int> watchlistRepository,
            IWatchlistAppService watchlistAppService)
        {
            _notificationRepository = notificationRepository;
            _watchlistRepository = watchlistRepository;
            _watchlistAppService = watchlistAppService;
        }

        public async Task GenerateNotificationsAsync()
        {
            var seriesWithChanges = await _watchlistAppService.GetSeriesWithChangesAsync();

            foreach (var series in seriesWithChanges)
            {
                var notification = new Notification
                {
                    Description = $"La serie '{series.Name}' ha tenido cambios recientes.",
                    Type = "Email",
                    WasRead = false
                };

                await _notificationRepository.InsertAsync(notification);
            }
        }
    }
}
