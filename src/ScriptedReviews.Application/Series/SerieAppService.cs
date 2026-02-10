using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Text.Json;

namespace ScriptedReviews.Series
{
    public class SerieAppService : CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {

        private readonly ISeriesApiService _seriesApiService;

        // API Key de OMDB 
        private const string OmdbApiKey = "c3034380";

        public SerieAppService(IRepository<Serie, int> repository, ISeriesApiService seriesApiService) : base(repository)
        { 
            _seriesApiService = seriesApiService;
        }

        [HttpGet("api/app/serie/search")]
        public async Task<ICollection<SerieDto>> SearchAsync(string? title, string? genre)
        {
            return await _seriesApiService.GetSeriesAsync(title, genre);
        }

        [HttpPost("api/app/serie/importar-serie")]
        public async Task<SerieDto> ImportarSerieAsync(string imdbId)
        {
            return await _seriesApiService.ImportarSerieAsync(imdbId);
        }
    }
}

