using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;
using Volo.Abp;
using ScriptedReviews.Series;

namespace ScriptedReviews.Ratings
{
    public class RatingAppService : ApplicationService, IRatingAppService
    {
        private readonly IRepository<Rating, int> _ratingRepository;
        private readonly IRepository<Serie, int> _serieRepository;

        public RatingAppService(
            IRepository<Rating, int> ratingRepository,
            IRepository<Serie, int> serieRepository)
        {
            _ratingRepository = ratingRepository;
            _serieRepository = serieRepository;
        }

        public async Task<RatingDto> RateSeriesAsync(CreateRatingDto input)
        {
            // Verificar que la serie pertenece al usuario
            var series = await _serieRepository.FirstOrDefaultAsync(s => s.Id == input.SeriesId);
            if (series == null)
            {
                throw new UserFriendlyException("La serie no fue encontrada.");
            }

            // Verificar si el usuario autenticado es el propietario de la serie
            if (series.UserId != CurrentUser.Id)
            {
                throw new UserFriendlyException("No puedes calificar una serie que no te pertenece.");
            }

            // Crear la calificación
            var Rating = new Rating
            {
                UserId = CurrentUser.Id.Value,
                SeriesId = input.SeriesId,
                RatingNumber = input.Rating,
                Comment = input.Comment
            };

            // Insertar en la base de datos
            await _ratingRepository.InsertAsync(Rating);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Mapear a DTO
            return ObjectMapper.Map<Rating, RatingDto>(Rating);
        }
    }
}
