using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

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

        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            var userId = CurrentUser.GetId();

            var notifications = await _notificationRepository.GetListAsync(
                n => n.UserId == userId
            );

            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            notification.WasRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }
        
        public async Task GenerateNotificationsAsync()
        {
            // 1. Obtenemos las watchlists que tienen cambios
            var watchlistsWithChanges = await _watchlistAppService.GetSeriesWithChangesAsync();

            // 2. Primer Bucle: Recorremos cada Lista de Seguimiento
            foreach (var watchlist in watchlistsWithChanges)
            {
                // 3. Segundo Bucle: Recorremos las SERIES que están DENTRO de esa lista
                // Asumimos que 'watchlist.Series' trae las series afectadas o todas las de la lista
                if (watchlist.Series != null)
                {
                    foreach (var serie in watchlist.Series)
                    {
                        var notification = new Notification
                        {
                            // AHORA SÍ: Usamos el nombre de la SERIE, no de la lista
                            // Puedes incluso mencionar en qué lista estaba:
                            Description = $"La serie '{serie.Title}' (en tu lista '{watchlist.Name}') ha tenido cambios recientes.",
                            Type = "Email",
                            WasRead = false
                        };

                        await _notificationRepository.InsertAsync(notification);
                    }
                }
            }
        }
    }
}
