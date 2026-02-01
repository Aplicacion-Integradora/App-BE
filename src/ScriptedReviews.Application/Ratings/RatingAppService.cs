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
using ScriptedReviews.Users;

namespace ScriptedReviews.Ratings
{
    // [Authorize] obliga a que el usuario esté logueado para usar esto
    [Authorize]
    public class RatingAppService : ScriptedReviewsAppService, IRatingAppService
    {
        private readonly IRepository<Rating, int> _ratingRepository;

        public RatingAppService(IRepository<Rating, int> ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<RatingDto> UpdateRatingAsync(int seriesId, UpdateRatingDto input)
        {
            // 1. Obtener el ID del usuario actual
            var currentUserId = CurrentUser.Id;
            // El null check es opcional si usamos [Authorize]
            if (currentUserId == null)
            {
                throw new UserFriendlyException("Debes iniciar sesión para editar una calificación.");
            }

            // 2. Buscar la calificación existente en la BD
            // Buscamos una fila que coincida con la Serie Y con el Usuario
            var rating = await _ratingRepository.FirstOrDefaultAsync(r => r.SeriesId == seriesId && r.UserId == currentUserId);

            if (rating == null)
            {
                throw new UserFriendlyException("No has calificado esta serie, por lo que no puedes editarla.");
            }

            // 3. Actualizar los datos 
            rating.RatingNumber = input.Rating;
            rating.Comment = input.Comment;

            // 4. Guardar los cambios
            await _ratingRepository.UpdateAsync(rating);

            // 5. Devolver el resultado convertido a DTO
            return ObjectMapper.Map<Rating, RatingDto>(rating);
        }
    }
}


    public class RatingAppService : ApplicationService, IRatingAppService
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

        public async Task<RatingDto> RateAsync(CreateRatingDto input)
        {
            // Verificar que la serie pertenece al usuario
            var series = await _serieRepository.GetAsync(s => s.Id == input.SeriesId);
            if (series == null)
            {
                throw new UserFriendlyException("La serie no fue encontrada.");
            }

            //Verificar que el usuario no sea nulo
            /*if (users.Id == null)
            {
                throw new UserFriendlyException("Debes estar logueado para calificar.");
            }*/

            // Verificar si el usuario autenticado es el propietario de la serie
            if (series.UserId != CurrentUser.Id.Value)
            {
                throw new UserFriendlyException("No puedes calificar una serie que no te pertenece.");
            }

            // Verificar si la serie está en la watchlist del usuario
            // se podría sacar el usuario que tiene la primera serie de la watchlist, ya que todas van a ser propiedad del mismo usuario
            // y comparar este con el usuario actual (current user)
            // quizás no sea necesario ya que arriba se verifica que la serie pertenezca al usuario y que el usuario autenticado es el
            // propietario de la serie
            var rating = ObjectMapper.Map<CreateRatingDto, Rating>(input);

            rating.UserId = CurrentUser.Id.Value;

            await _ratingRepository.InsertAsync(rating);

            // Devolver el DTO
            return ObjectMapper.Map<Rating, RatingDto>(rating);
        }
        public async Task<List<RatingDto>> GetListAsync()
        {
            var ratings = await _ratingRepository.GetListAsync();
            return ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
        }
    }
}
