using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services; 

namespace ScriptedReviews.Series
{
    public interface ISerieAppService: ICrudAppService<SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>
    {
        // Método para la Operación 2.2
        Task<SerieDto> ImportarSerieAsync(string titulo);
    }
}
