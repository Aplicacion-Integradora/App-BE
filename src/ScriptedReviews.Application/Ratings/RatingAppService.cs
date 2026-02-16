using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using ScriptedReviews.Ratings;
using ScriptedReviews.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;


namespace ScriptedReviews.Ratings
{
    [Authorize]
    public class RatingAppService : ScriptedReviewsAppService, IRatingAppService
    {
        private readonly IRepository<Rating, Guid> _ratingRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public RatingAppService(
            IRepository<Rating, Guid> ratingRepository,
            IRepository<Serie, int> serieRepository,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _ratingRepository = ratingRepository;
            _serieRepository = serieRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<RatingDto> RateAsync(CreateRatingDto input)
        {
            // Verifica que la serie pertenece al usuario
            var series = await _serieRepository.GetAsync(s => s.Id == input.SeriesId);
            if (series == null)
            {
                throw new UserFriendlyException("La serie no fue encontrada.");
            }

            // Verifica si el usuario autenticado es el propietario de la serie
            if (series.UserId != CurrentUser.Id.Value)
            {
                throw new UserFriendlyException("No puedes calificar una serie que no te pertenece.");
            }

            // Verifica si la serie está en la watchlist del usuario
            var rating = ObjectMapper.Map<CreateRatingDto, Rating>(input);

            rating.UserId = CurrentUser.Id.Value;

            await _ratingRepository.InsertAsync(rating);

            // Devuelve el DTO
            return ObjectMapper.Map<Rating, RatingDto>(rating);
        }
        public async Task<List<RatingDto>> GetListAsync()
        {
            var ratings = await _ratingRepository.GetListAsync();
            return ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
        }

        [Authorize]
        public async Task<RatingDto> UpdateRatingAsync(int seriesId, UpdateRatingDto input)
        {
            // Chequea que el usuario no sea nulo, y en caso de no serlo, se prosigue con la modificación de calificacion
            if (CurrentUser.Id == null)

            {
                throw new UserFriendlyException("Debes iniciar sesión para editar una calificación.");
            }
            var currentUserId = CurrentUser.Id.Value;

            // Busca la calificación existente en la BD
            // Busca una fila que coincida con la Serie y con el Usuario
            var rating = await _ratingRepository.FirstOrDefaultAsync(r => r.SeriesId == seriesId && r.UserId == currentUserId);

            if (rating == null)
            {
                throw new UserFriendlyException("No has calificado esta serie, por lo que no puedes editarla.");
            }

            // Actualiza los datos 
            rating.RatingNumber = input.RatingNumber;
            rating.Comment = input.Comment;

            // Guarda los cambios
            await _ratingRepository.UpdateAsync(rating);

            // Devuelve el resultado convertido a DTO
            return ObjectMapper.Map<Rating, RatingDto>(rating);
        }
    }
}
