using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScriptedReviews.Notifications;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace ScriptedReviews.BackgroundWorkers
{
    public class NotificationBackgroundWorker
    : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public NotificationBackgroundWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = 43200000;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            // Resuelve dependencias
            var serieRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Serie, int>>();
            var watchlistRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Watchlist, int>>();
            var notificationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Notification, int>>();
            var seriesApiService = workerContext.ServiceProvider.GetRequiredService<ISeriesApiService>();

            // Obtiene todas las series locales
            var seriesLocales = await serieRepository.GetListAsync(includeDetails: true);

            foreach (var serieLocal in seriesLocales)
            {
                // Si no tiene ID de IMDB, no podemos chequear nada
                if (string.IsNullOrEmpty(serieLocal.ImdbId)) continue;

                try
                {
                    // Consulta a OMDB la información nueva
                    var infoApi = await seriesApiService.ImportarSerieAsync(serieLocal.ImdbId);

                    if (infoApi == null) continue;

                    // Lógica de comparación
                    int temporadasEnApi = infoApi.TotalSeasons;
                    int temporadasLocales = serieLocal.TotalSeasons;

                    // Si la API dice que hay más temporadas de las que tenemos, prosigue para notificar
                    if (temporadasEnApi > temporadasLocales)
                    {
                        // Busca usuarios interesados (que tienen la serie en su Watchlist)
                        var queryable = await watchlistRepository.WithDetailsAsync(x => x.Series);
                        var watchlistsConLaSerie = queryable
                            .Where(w => w.Series.Any(s => s.Id == serieLocal.Id))
                            .ToList();

                        // Crea notificaciones para esos usuarios y marca HasChanges = true
                        foreach (var watchlist in watchlistsConLaSerie)
                        {
                            // Verifica si ya existe una notificación no leída para esta serie
                            var existingNotification = await notificationRepository.FirstOrDefaultAsync(
                                n => n.UserId == watchlist.UserId 
                                     && n.Description.Contains(serieLocal.Title) 
                                     && !n.WasRead);

                            if (existingNotification == null)
                            {
                                await notificationRepository.InsertAsync(new Notification
                                {
                                    UserId = watchlist.UserId,
                                    Description = $"¡Nueva temporada disponible! '{serieLocal.Title}' ahora tiene {temporadasEnApi} temporadas.",
                                    Type = "NewSeason",
                                    SentTime = DateTime.Now,
                                    WasRead = false
                                });
                            }

                            // Marca la watchlist con cambios (HasChanges = true)
                            watchlist.HasChanges = true;
                            await watchlistRepository.UpdateAsync(watchlist);
                        }

                        // Actualiza la serie local para evitar notificaciones duplicadas
                        serieLocal.TotalSeasons = temporadasEnApi;
                        await serieRepository.UpdateAsync(serieLocal);
                    }
                }
                catch (Exception ex)
                {
                    // Para que un error en una serie no detenga todo el proceso
                    Console.WriteLine($"Error procesando serie {serieLocal.Title}: {ex.Message}");
                }
            }
        }
    }
}
