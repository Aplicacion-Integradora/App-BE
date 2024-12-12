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
        private readonly IRepository<Watchlist, int> _watchlistRepository;  // Lista de seguimiento
        private readonly iWatchlistAppService _watchlistAppService;

        public NotificationAppService(
            IRepository<Notification, int> notificationRepository,
            IRepository<Watchlist, int> watchlistRepository)
        {
            _notificationRepository = notificationRepository;
            _watchlistRepository = watchlistRepository;
        }

        public async Task GenerateNotificationsAsync()
        {
            // Obtener las series de la lista de seguimiento con cambios
            var seriesWithChanges = await _watchlistAppService.GetSeriesWithChangesAsync();
            //var seriesWithChanges = await WatchlistAppService.GetSeriesWithChangesAsync(s => s.HasChanges);

            foreach (var series in seriesWithChanges)
            {
                var notification = new Notification
                {
                    Description = $"La serie '{series.Name}' ha tenido cambios recientes.",
                    Type = "Email",  // Ejemplo de método, puede ser dinámico
                    WasRead = false
                };

                await _notificationRepository.InsertAsync(notification);
            }

            await CurrentUnitOfWork.SaveChangesAsync();  // Persistir en la base de datos
        }
    }
}
