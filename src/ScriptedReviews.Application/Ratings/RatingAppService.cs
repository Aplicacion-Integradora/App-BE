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
using Volo.Abp.Identity;
using Volo.Abp;
using ScriptedReviews.Series;
using ScriptedReviews.Users;

namespace ScriptedReviews.Ratings
{
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
