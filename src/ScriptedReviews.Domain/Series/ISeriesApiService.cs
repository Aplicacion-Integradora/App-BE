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
        // Task<List<SerieDto>> GetListAsync();
                                                                                       
        // Task<SerieDto> CreateAsync(SerieCreateDto input);
                                                                                       
        // Task<SerieDto> UpdateAsync(int id, SerieUpdateDto input);
                                                                                       
        // Task DeleteAsync(string title);
    }
}
