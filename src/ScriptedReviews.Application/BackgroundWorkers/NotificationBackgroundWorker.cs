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
            // 1. Resolver dependencias (Como es Singleton, las pedimos al contexto)
            var serieRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Serie, int>>();
            var watchlistRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Watchlist, int>>();
            var notificationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Notification, int>>();
            var seriesApiService = workerContext.ServiceProvider.GetRequiredService<ISeriesApiService>();

            // 2. Obtener todas las series locales
            // (Nota: Si tienes miles, usa Paginación. Para empezar, esto sirve.)
            var seriesLocales = await serieRepository.GetListAsync(includeDetails: true);

            foreach (var serieLocal in seriesLocales)
            {
                // Si no tiene ID de IMDB, no podemos chequear nada
                if (string.IsNullOrEmpty(serieLocal.ImdbId)) continue;

                try
                {
                    // 3. Consultar a OMDB la información FRESCA
                    // Usamos el método que acabas de arreglar
                    var infoApi = await seriesApiService.ImportarSerieAsync(serieLocal.ImdbId);

                    if (infoApi == null) continue;

                    // 4. LÓGICA DE COMPARACIÓN (El corazón de la notificación)
                    int temporadasEnApi = infoApi.TotalSeasons;

                    // Usamos TotalSeasons como fuente canónica
                    int temporadasLocales = serieLocal.TotalSeasons;

                    // Si la API dice que hay MÁS temporadas de las que tenemos... ¡Noticia!
                    if (temporadasEnApi > temporadasLocales)
                    {
                        // A. Buscar usuarios interesados (que tienen la serie en su Watchlist)
                        var queryable = await watchlistRepository.WithDetailsAsync(x => x.Series);
                        var watchlistsConLaSerie = queryable
                            .Where(w => w.Series.Any(s => s.Id == serieLocal.Id))
                            .ToList();

                        // B. Crear notificaciones para esos usuarios y marcar HasChanges
                        foreach (var watchlist in watchlistsConLaSerie)
                        {
                            // Verificar si ya existe una notificación no leída para esta serie
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

                            // Marcar la watchlist con cambios
                            watchlist.HasChanges = true;
                            await watchlistRepository.UpdateAsync(watchlist);
                        }

                        // C. Actualizar la serie local para evitar notificaciones duplicadas
                        serieLocal.TotalSeasons = temporadasEnApi;
                        await serieRepository.UpdateAsync(serieLocal);
                    }
                }
                catch (Exception ex)
                {
                    // Importante: Que un error en una serie no detenga todo el proceso
                    Console.WriteLine($"Error procesando serie {serieLocal.Title}: {ex.Message}");
                }
            }
        }
    }
}
