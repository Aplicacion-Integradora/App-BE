using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Series
{
    public interface ISeriesApiService
    {
        Task<ICollection<SerieDto>> GetSeriesAsync(string title, string genre);

        Task<SerieDto> ImportarSerieAsync(string imdbId);
    }
}
