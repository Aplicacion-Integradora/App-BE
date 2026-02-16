using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists.Dtos;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Application.Dtos;
using System.Linq.Expressions;


namespace ScriptedReviews.Watchlists
{
    public class WatchlistAppService : ApplicationService, IWatchlistAppService

    {
        private readonly IRepository<ScriptedReviews.Watchlists.Watchlist, int> _watchlistRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<WatchlistAppService> _logger;

        public WatchlistAppService(
            ILogger<WatchlistAppService> logger,
            IRepository<ScriptedReviews.Watchlists.Watchlist, int> watchlistRepository,
            IRepository<Serie, int> serieRepository,
            IMapper mapper,
            ICurrentUser currentUser)
        {
            _watchlistRepository = watchlistRepository;
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
                var query = await _watchlistRepository.GetQueryableAsync();

                var watchlist = await query
                    .Include(w => w.Series) // Carga la lista List<Serie>
                    .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

                if (watchlist == null || watchlist.Series == null)
                {
                    return new List<SerieDto>();
                }

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
            // Valida que la serie exista (persistencia interna)
            var serie = await _serieRepository.FindAsync(serieId);
            if (serie == null)
            {
                throw new UserFriendlyException("La serie no existe en la base de datos.");
            }

            // Obtiene la watchlist del usuario actual
            var query = await _watchlistRepository.GetQueryableAsync();
            var watchlist = await query
                .Include(w => w.Series)
                .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

            // Si no tiene lista, se la crea
            if (watchlist == null)
            {
                watchlist = new ScriptedReviews.Watchlists.Watchlist(CurrentUserId)
                {
                    Name = "Mi Lista" // Nombre por defecto
                };
                await _watchlistRepository.InsertAsync(watchlist);
            }

            // Agrega la serie si no está duplicada
            if (!watchlist.Series.Any(s => s.Id == serieId))
            {
                watchlist.Series.Add(serie);
                await _watchlistRepository.UpdateAsync(watchlist);
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
            var query = await _watchlistRepository.GetQueryableAsync();
            var watchlist = await query
                .Include(w => w.Series)
                .FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

            if (watchlist != null && watchlist.Series != null)
            {
                var serieToRemove = watchlist.Series.FirstOrDefault(s => s.Id == serieId);

                if (serieToRemove != null)
                {
                    watchlist.Series.Remove(serieToRemove);
                    await _watchlistRepository.UpdateAsync(watchlist);
                    _logger.LogInformation($"Serie {serieId} eliminada de la lista del usuario {CurrentUserId}");
                }
            }
        }

        // Método para obtener series con cambios
        public async Task<List<WatchlistDto>> GetSeriesWithChangesAsync()
        {
            var queryableSeries = await _watchlistRepository.GetQueryableAsync();
            var seriesWithChanges = queryableSeries
                .Where(s => s.HasChanges)
                .ToList();

            return ObjectMapper.Map<List<Watchlist>, List<WatchlistDto>>(seriesWithChanges);
        }

        // Método para limpiar el flag HasChanges del usuario actual
        public async Task ClearChangesAsync()
        {
            var query = await _watchlistRepository.GetQueryableAsync();
            var watchlist = await query.FirstOrDefaultAsync(w => w.UserId == CurrentUserId);

            if (watchlist != null && watchlist.HasChanges)
            {
                watchlist.HasChanges = false;
                await _watchlistRepository.UpdateAsync(watchlist);
                _logger.LogInformation($"HasChanges limpiado para el usuario {CurrentUserId}");
            }
        }
    }
}