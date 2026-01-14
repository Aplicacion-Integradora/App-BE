using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Necesario para .Include()
using Microsoft.Extensions.Logging;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace ScriptedReviews.Watchlist
{
    // Heredamos de ApplicationService e implementamos la interfaz
    public class WatchListAppService : ApplicationService, IWatchListAppService
    {
        // Usamos IRepository<Watchlist, int> para asegurar que compile si no tienes la interfaz personalizada
        private readonly IRepository<ScriptedReviews.Watchlists.Watchlist, int> _watchListRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<WatchListAppService> _logger;

        public WatchListAppService(
            ILogger<WatchListAppService> logger,
            IRepository<ScriptedReviews.Watchlists.Watchlist, int> watchListRepository, // Ajustado a genérico para seguridad
            IRepository<Serie, int> serieRepository,
            IMapper mapper,
            ICurrentUser currentUser)
        {
            _watchListRepository = watchListRepository;
            _serieRepository = serieRepository;
            _currentUser = currentUser;
            _mapper = mapper;
            _logger = logger;
        }

        // Propiedad auxiliar para obtener el ID del usuario de forma segura
        private Guid CurrentUserId
        {
            get
            {
                // Como el PDF dice que "no se requiere sistema de seguridad activo",
                // esto podría ser null si probamos sin login.
                // Aquí lanzamos un error amigable si intentan usarlo sin estar logueados.
                if (!_currentUser.Id.HasValue)
                {
                    throw new UserFriendlyException("Debes estar logueado (o simular un usuario) para ver tu lista.");
                }
                return _currentUser.Id.Value;
            }
        }

        // TAREA 3.1: Obtener las series de lista de seguimiento
        public async Task<List<SerieDto>> GetMyWatchlistAsync()
        {
            try
            {
                var query = await _watchListRepository.GetQueryableAsync();

                var watchlist = await query
                    .Include(w => w.Series) // Carga la lista List<Serie>
                    .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

                if (watchlist == null || watchlist.Series == null)
                {
                    return new List<SerieDto>();
                }

                // Usamos el _mapper explícito como tus compañeros
                return _mapper.Map<List<Serie>, List<SerieDto>>(watchlist.Series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la watchlist"); // Cumple con bitácora de errores
                throw;
            }
        }

        // TAREA 3.2: Agregar series a la lista de seguimiento
        public async Task AddSerieAsync(int serieId)
        {
            // 1. Validar que la serie exista (persistencia interna)
            var serie = await _serieRepository.FindAsync(serieId);
            if (serie == null)
            {
                throw new UserFriendlyException("La serie no existe en la base de datos.");
            }

            // 2. Obtener la watchlist del usuario actual
            var query = await _watchListRepository.GetQueryableAsync();
            var watchlist = await query
                .Include(w => w.Series)
                .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

            // 3. Si no tiene lista, se la creamos
            if (watchlist == null)
            {
                watchlist = new ScriptedReviews.Watchlists.Watchlist(CurrentUserId);
                await _watchListRepository.InsertAsync(watchlist);
            }

            // 4. Agregamos la serie si no está duplicada
            if (!watchlist.Series.Any(s => s.Id == serieId))
            {
                watchlist.Series.Add(serie);
                await _watchListRepository.UpdateAsync(watchlist);
                _logger.LogInformation($"Serie {serieId} agregada a la lista del usuario {CurrentUserId}");
            }
            else
            {
                throw new UserFriendlyException("La serie ya está en tu lista.");
            }
        }

        // TAREA 3.4: Eliminar series de la lista de seguimiento
        public async Task RemoveSerieAsync(int serieId)
        {
            var query = await _watchListRepository.GetQueryableAsync();
            var watchlist = await query
                .Include(w => w.Series)
                .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

            if (watchlist != null && watchlist.Series != null)
            {
                var serieToRemove = watchlist.Series.FirstOrDefault(s => s.Id == serieId);

                if (serieToRemove != null)
                {
                    watchlist.Series.Remove(serieToRemove);
                    await _watchListRepository.UpdateAsync(watchlist);
                    _logger.LogInformation($"Serie {serieId} eliminada de la lista del usuario {CurrentUserId}");
                }
            }
        }
    }
}