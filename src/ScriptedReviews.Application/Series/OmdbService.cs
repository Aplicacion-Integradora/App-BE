using AutoMapper.Internal.Mappers;
using Microsoft.Extensions.Configuration;
using ScriptedReviews.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace ScriptedReviews.Series
{
    public class OmdbService : ApplicationService, ISeriesApiService
    {
        private static readonly string apiKey = "a6754de0"; 
        private static readonly string baseUrl = "http://www.omdbapi.com/";

        private readonly IConfiguration _configuration;
        private readonly IRepository<Serie, int> _serieRepository; // Para arreglar el error de 'Repository'
        private readonly System.Net.Http.HttpClient _httpClient; // Asumo que usas esto

        public OmdbService(
            IConfiguration configuration,
            IRepository<Serie, int> serieRepository)
        {
            _configuration = configuration;
            _serieRepository = serieRepository;
            _httpClient = new System.Net.Http.HttpClient(); // O inyectar IHttpClientFactory idealmente
        }

        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string genre)
        {
            using HttpClient client = new HttpClient();

            List<SerieDto> series = new List<SerieDto>();

            string url = $"{baseUrl}?s={title}&apikey={apiKey}&type=series";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var searchResponse = JsonSerializer.Deserialize<SearchResponse>(jsonResponse);

                var seriesOmdb = searchResponse?.Search ?? new List<SerieOmdb>();

                foreach (var serieOmdb in seriesOmdb)
                {
                    series.Add(new SerieDto { 
                        Title = serieOmdb.Title,
                        Image = serieOmdb.Image,
                        Genre = serieOmdb.Genre,
                        ReleaseDate = serieOmdb.ReleaseDate,
                        Duration = serieOmdb.Duration,
                        Country = serieOmdb.Country,
                        Director = serieOmdb.Director,
                        Cast = serieOmdb.Cast,
                        Writer = serieOmdb.Writer
                    });
                }

                return series;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Se ha producido un error en la búsqueda de la serie", e);
            }
        }

        private class SearchResponse
        {
            [JsonPropertyName("Search")]
            public List<SerieOmdb> Search { get; set; }
        }
        private class SerieOmdb
        {
            public string Title { get; set; }

            public string Image { get; set; }

            public string Genre { get; set; }

            public string ReleaseDate { get; set; }

            public string Duration { get; set; }

            public string Country { get; set; }

            public string Director { get; set; }

            public string Cast { get; set; }

            public string Writer { get; set; }
        }

        public async Task<SerieDto> ImportarSerieAsync(string titulo)
        {
            var apiKey = _configuration["OmdbApiKey"];
            // 1. Conectar con OMDB (Ahora es el primer paso para obtener el ID único)
            string url = $"http://www.omdbapi.com/?t={titulo}&apikey={apiKey}";
            OmdbDto datosExternos;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new UserFriendlyException("Error al conectar con el servidor de películas.");
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                datosExternos = JsonSerializer.Deserialize<OmdbDto>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (datosExternos == null || datosExternos.Response == "False")
            {
                throw new UserFriendlyException($"No se encontró la serie: {titulo}");
            }

            // 2. Verificar si ya existe la serie en la BD usando el ID ÚNICO (ImdbId)
            // Esto evita duplicados si la serie cambia de nombre o si el usuario escribe diferente.
            var existente = await _serieRepository.FirstOrDefaultAsync(x => x.ImdbId == datosExternos.imdbID);

            if (existente != null)
            {
                return ObjectMapper.Map<Serie, SerieDto>(existente);
            }

            // 3. Mapeo Manual: Convertir datos de OMDB a tu Entidad 'Serie'
            var nuevaSerie = new Serie
            {
                Title = datosExternos.Title ?? "Sin Título",
                ImdbId = datosExternos.imdbID, // Campo agregado para persistir la identidad única
                Genre = datosExternos.Genre ?? "Desconocido",

                Director = datosExternos.Director != null && datosExternos.Director != "N/A"
                           ? datosExternos.Director
                           : "Desconocido",

                Writer = datosExternos.Writer != null && datosExternos.Writer != "N/A"
                           ? datosExternos.Writer
                           : "Desconocido",

                Language = datosExternos.Language != null && datosExternos.Language != "N/A"
                           ? datosExternos.Language
                           : "Original", // Valor por defecto para evitar el error NULL

                ReleaseDate = datosExternos.Year ?? "N/A",
                Duration = datosExternos.Runtime ?? "N/A",
                Image = datosExternos.Poster != "N/A" ? datosExternos.Poster : null,
                Country = datosExternos.Country ?? "Desconocido",
                Rating = datosExternos.imdbRating ?? "0",

                Cast = datosExternos.Actors?.Length > 200
                        ? datosExternos.Actors.Substring(0, 200)
                        : (datosExternos.Actors ?? "Desconocido"),

                Description = datosExternos.Plot?.Length > 500
                              ? datosExternos.Plot.Substring(0, 500)
                              : (datosExternos.Plot ?? "Sin descripción")
            };

            // 4. Guardar en Base de Datos 
            var serieInsertada = await _serieRepository.InsertAsync(nuevaSerie, autoSave: true);

            return ObjectMapper.Map<Serie, SerieDto>(serieInsertada);
        }
    }
}