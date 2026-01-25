using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Ratings
{
    public interface IRatingAppService : IApplicationService
    {
        // Método que permitirá modificar la calificación desde la API
        Task<RatingDto> UpdateRatingAsync(int seriesId, UpdateRatingDto input);
    }
}