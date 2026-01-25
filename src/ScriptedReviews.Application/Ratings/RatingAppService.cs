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