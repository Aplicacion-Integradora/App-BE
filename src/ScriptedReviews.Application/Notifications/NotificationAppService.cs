using Microsoft.AspNetCore.Authorization;
using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace ScriptedReviews.Notifications
{
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<Watchlist, int> _watchlistRepository;
        private readonly IWatchlistAppService _watchlistAppService;
        private readonly ICurrentUser _currentUser;

        public NotificationAppService(
            IRepository<Notification, int> notificationRepository,
            IRepository<Watchlist, int> watchlistRepository,
            IWatchlistAppService watchlistAppService,
            ICurrentUser currentUser)
        {
            _notificationRepository = notificationRepository;
            _watchlistRepository = watchlistRepository;
            _watchlistAppService = watchlistAppService;
            _currentUser = currentUser;
        }

        [Authorize]
        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            var currentUserId = _currentUser.Id;
            
            if (currentUserId == null)
            {
                throw new UserFriendlyException("Debes estar logueado para ver tus notificaciones.");
            }

            // Para ver las notificaciones en orden
            var queryable = await _notificationRepository.GetQueryableAsync();

            var query = queryable
                .Where(n => n.UserId == currentUserId)
                .OrderByDescending(n => n.SentTime); // Asumiendo que heredas de AuditedAggregateRoot

            var notifications = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
        }

        [Authorize]
        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            if (notification.UserId != _currentUser.Id)
            {
                // Lanzamos un error genérico (o 404) para no dar pistas
                throw new UserFriendlyException("No tienes permiso para modificar esta notificación.");
            }

            notification.WasRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }
        
        public async Task GenerateNotificationsAsync()
        {
            var queryable = await _watchlistRepository.WithDetailsAsync(x => x.Series);

            // 2. Filtramos
            var query = queryable.Where(w => w.HasChanges);

            // 3. Ejecutamos
            var watchlists = await AsyncExecuter.ToListAsync(query);

            // Recorremos cada watchlist
            foreach (var watchlist in watchlists)
            {
                // Recorremos las series dentro de cada watchlist
                if (watchlist.Series != null)
                {
                    foreach (var serie in watchlist.Series)
                    {
                        var notification = new Notification
                        {
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
