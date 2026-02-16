using Microsoft.AspNetCore.Authorization;
using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<Watchlist, int> _watchlistRepository;
        private readonly ISeriesApiService _seriesApiService;
        private readonly ICurrentUser _currentUser;

        public NotificationAppService(
            IRepository<Notification, int> notificationRepository,
            IRepository<Serie, int> serieRepository,
            IRepository<Watchlist, int> watchlistRepository,
            ISeriesApiService seriesApiService,
            ICurrentUser currentUser)
        {
            _notificationRepository = notificationRepository;
            _serieRepository = serieRepository;
            _watchlistRepository = watchlistRepository;
            _seriesApiService = seriesApiService;
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
                .OrderByDescending(n => n.SentTime);

            var notifications = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
        }

        [Authorize]
        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            if (notification.UserId != _currentUser.Id)
            {
                // Lanza un error genérico (o 404)
                throw new UserFriendlyException("No tienes permiso para modificar esta notificación.");
            }

            notification.WasRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }
        
        public async Task GenerateNotificationsAsync()
        {
            // Trae las series locales
            var seriesLocales = await _serieRepository.GetListAsync(includeDetails: true);

            foreach (var serieLocal in seriesLocales)
            {
                if (string.IsNullOrEmpty(serieLocal.ImdbId)) continue;

                // Consulta a la API
                var infoApi = await _seriesApiService.ImportarSerieAsync(serieLocal.ImdbId);

                if (infoApi == null) continue;

                // Compara usando el campo TotalSeasons de la entidad
                int temporadasEnApi = infoApi.TotalSeasons;
                int temporadasLocales = serieLocal.TotalSeasons;

                if (temporadasEnApi > temporadasLocales)
                {
                    // Busca los usuarios para notificarlos
                    var queryable = await _watchlistRepository.WithDetailsAsync(x => x.Series);
                    var watchlists = queryable
                        .Where(w => w.Series.Any(s => s.Id == serieLocal.Id))
                        .ToList();

                    foreach (var watchlist in watchlists)
                    {
                        // Verifica si ya existe una notificación no leída para esta serie
                        var existingNotification = await _notificationRepository.FirstOrDefaultAsync(
                            n => n.UserId == watchlist.UserId 
                                 && n.Description.Contains(serieLocal.Title) 
                                 && !n.WasRead);

                        if (existingNotification == null)
                        {
                            await _notificationRepository.InsertAsync(new Notification
                            {
                                UserId = watchlist.UserId,
                                Description = $"¡Nueva temporada disponible! '{serieLocal.Title}' tiene nueva temporada.",
                                Type = "NewSeason",
                                SentTime = DateTime.Now,
                                WasRead = false
                            });
                        }

                        // Marca la watchlist con cambios
                        watchlist.HasChanges = true;
                        await _watchlistRepository.UpdateAsync(watchlist);
                    }

                    // Actualiza la serie local para evitar notificaciones duplicadas
                    serieLocal.TotalSeasons = temporadasEnApi;
                    await _serieRepository.UpdateAsync(serieLocal);
                }
            }
        }
    }
}
