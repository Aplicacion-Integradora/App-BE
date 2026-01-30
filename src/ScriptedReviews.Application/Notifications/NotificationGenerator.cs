using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ScriptedReviews.Notifications
{
    public class NotificationGenerator : DomainService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IWatchlistAppService _watchlistAppService;

        public NotificationGenerator(
            IRepository<Notification, int> notificationRepository,
            IWatchlistAppService watchlistAppService)
        {
            _notificationRepository = notificationRepository;
            _watchlistAppService = watchlistAppService;
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
            }
        }
    }
}
