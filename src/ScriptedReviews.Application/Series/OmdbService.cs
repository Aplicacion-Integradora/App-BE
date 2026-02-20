using AutoMapper.Internal.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private static readonly string apiKey = "c3034380"; 
        private static readonly string baseUrl = "http://www.omdbapi.com/";

        private readonly IConfiguration _configuration;
        private readonly IRepository<Serie, int> _serieRepository; 
        private readonly IHttpClientFactory _httpClientFactory; 

        public OmdbService(
            IConfiguration configuration,
            IRepository<Serie, int> serieRepository,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _serieRepository = serieRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string genre)
        {
            var client = _httpClientFactory.CreateClient();

            List<SerieDto> series = new List<SerieDto>();

            string url = $"{baseUrl}?s={title}&apikey={apiKey}&type=series";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var searchResponse = JsonSerializer.Deserialize<SearchResponse>(jsonResponse);

                var seriesOmdb = searchResponse?.Search ?? new List<SerieOmdb>();

                if (!string.IsNullOrEmpty(genre))
                {
                    // Se limita a 10 peticiones para que no se sature
                    var topSeries = seriesOmdb.Take(10).ToList();
                    
                    foreach (var item in topSeries)
                    {
                        // Busca detalles por ID
                        string detailUrl = $"{baseUrl}?i={item.imdbID}&apikey={apiKey}";
                        var detailResponse = await client.GetAsync(detailUrl);
                        if (detailResponse.IsSuccessStatusCode)
                        {
                            var detailJson = await detailResponse.Content.ReadAsStringAsync();
                            var details = JsonSerializer.Deserialize<OmdbDto>(detailJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (details != null && details.Genre != null && 
                                details.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
                            {
                                // Coincide el género, agregamos con datos completos
                                series.Add(new SerieDto
                                {
                                    Title = details.Title,
                                    ImdbId = details.imdbID,
                                    Image = details.Poster != "N/A" ? details.Poster : null,
                                    Genre = details.Genre,
                                    ReleaseDate = details.Year,
                                    Duration = details.Runtime,
                                    Country = details.Country,
                                    Director = details.Director,
                                    Cast = details.Actors,
                                    Writer = details.Writer,
                                    Description = details.Plot, 
                                    Rating = details.imdbRating
                                });
                            }
                        }
                    }
                }
                else
                {
                    foreach (var serieOmdb in seriesOmdb)
                    {
                        series.Add(new SerieDto
                        {
                            Title = serieOmdb.Title,
                            ImdbId = serieOmdb.imdbID,
                            Image = serieOmdb.Image != "N/A" ? serieOmdb.Image : null,
                            Genre = serieOmdb.Genre,
                            ReleaseDate = serieOmdb.ReleaseDate,
                            Duration = serieOmdb.Duration,
                            Country = serieOmdb.Country,
                            Director = serieOmdb.Director,
                            Cast = serieOmdb.Cast,
                            Writer = serieOmdb.Writer
                        });
                    }
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
            
            public string imdbID { get; set; }

            [JsonPropertyName("Poster")]
            public string Image { get; set; }

            public string Genre { get; set; }

            [JsonPropertyName("Year")]
            public string ReleaseDate { get; set; }

            public string Duration { get; set; }

            public string Country { get; set; }

            public string Director { get; set; }

            public string Cast { get; set; }

            public string Writer { get; set; }
        }

        // DTOs para respuesta de temporadas de OMDB
        private class OmdbSeasonResponse
        {
            public string Title { get; set; }
            public string Season { get; set; }
            public string totalSeasons { get; set; }
            public List<OmdbEpisode> Episodes { get; set; }
            public string Response { get; set; }
        }

        private class OmdbEpisode
        {
            public string Title { get; set; }
            public string Released { get; set; }
            public string Episode { get; set; }
            public string imdbRating { get; set; }
            public string imdbID { get; set; }
        }

        // Obtiene todas las temporadas de una serie desde OMDB API
        private async Task<List<ScriptedReviews.Seasons.Season>> FetchSeasonsAsync(string imdbId, int totalSeasons)
        {
            var seasons = new List<ScriptedReviews.Seasons.Season>();
            var apiKey = _configuration["OMDB:ApiKey"];

            for (int seasonNum = 1; seasonNum <= totalSeasons; seasonNum++)
            {
                try
                {
                    string url = $"http://www.omdbapi.com/?i={imdbId}&Season={seasonNum}&apikey={apiKey}";
                    Logger.LogInformation("Fetching season {SeasonNum} from OMDB: {Url}", seasonNum, url);

                    using (var client = _httpClientFactory.CreateClient())
                    {
                        var response = await client.GetAsync(url);
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogWarning("Failed to fetch season {SeasonNum}, status: {StatusCode}", seasonNum, response.StatusCode);
                            continue;
                        }

                        var jsonResult = await response.Content.ReadAsStringAsync();
                        var seasonData = JsonSerializer.Deserialize<OmdbSeasonResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (seasonData == null || seasonData.Response == "False" || seasonData.Episodes == null)
                        {
                            Logger.LogWarning("No episode data for season {SeasonNum}", seasonNum);
                            continue;
                        }

                        // Crea descripción a partir de los títulos de los episodios
                        var episodeTitles = seasonData.Episodes.Select(e => e.Title).ToList();
                        var description = string.Join(", ", episodeTitles.Take(5));
                        if (episodeTitles.Count > 5)
                        {
                            description += $"... y {episodeTitles.Count - 5} más";
                        }

                        // Obtiene fecha de lanzamiento del primer episodio
                        var firstEpisode = seasonData.Episodes.FirstOrDefault();
                        var releaseDate = firstEpisode?.Released ?? "N/A";

                        var season = new ScriptedReviews.Seasons.Season
                        {
                            Number = seasonNum,
                            Description = description,
                            ReleaseDate = releaseDate,
                            Chapters = seasonData.Episodes.Count.ToString()
                        };

                        seasons.Add(season);
                        Logger.LogInformation("Season {SeasonNum} fetched: {Chapters} episodes", seasonNum, season.Chapters);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error fetching season {SeasonNum}", seasonNum);
                }
            }

            return seasons;
        }

        public async Task<SerieDto> ImportarSerieAsync(string imdbId)
        {
            Logger.LogInformation("=== ImportarSerieAsync STARTED for imdbId: {ImdbId} ===", imdbId);
            
            try
            {
                var apiKey = _configuration["OMDB:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    apiKey = "c3034380";
                }
                Logger.LogInformation("API Key from config: {ApiKey}", string.IsNullOrEmpty(apiKey) ? "NULL/EMPTY" : apiKey.Substring(0, Math.Min(4, apiKey.Length)) + "...");
                
                // Conecta con OMDB usando ImdbId
                string url = $"http://www.omdbapi.com/?i={imdbId}&apikey={apiKey}";
                Logger.LogInformation("OMDB URL: {Url}", url);
                
                OmdbDto datosExternos;

                using (var client = _httpClientFactory.CreateClient())
                {
                    Logger.LogInformation("Sending HTTP request to OMDB...");
                    var response = await client.GetAsync(url);
                    Logger.LogInformation("HTTP Response Status: {StatusCode}", response.StatusCode);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.LogError("HTTP request failed with status: {StatusCode}", response.StatusCode);
                        throw new UserFriendlyException("Error al conectar con el servidor de series.");
                    }

                    var jsonResult = await response.Content.ReadAsStringAsync();
                    Logger.LogInformation("OMDB JSON Response (first 200 chars): {Json}", jsonResult.Substring(0, Math.Min(200, jsonResult.Length)));
                    
                    datosExternos = JsonSerializer.Deserialize<OmdbDto>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Logger.LogInformation("Deserialized OmdbDto - Title: {Title}, Response: {Response}, imdbID: {ImdbId}", 
                        datosExternos?.Title ?? "NULL", 
                        datosExternos?.Response ?? "NULL",
                        datosExternos?.imdbID ?? "NULL");
                }

                if (datosExternos == null || datosExternos.Response == "False")
                {
                    Logger.LogWarning("Serie not found in OMDB: {ImdbId}", imdbId);
                    throw new UserFriendlyException($"No se encontró la serie con ID: {imdbId}");
                }

                // Verifica si ya existe la serie en la BDD usando el ImdbId
                Logger.LogInformation("Checking if serie exists in DB with ImdbId: {ImdbId}", datosExternos.imdbID);
                var existente = await _serieRepository.FirstOrDefaultAsync(x => x.ImdbId == datosExternos.imdbID);

                if (existente != null)
                {
                    Logger.LogInformation("Serie already exists in DB with Id: {Id}", existente.Id);
                    return ObjectMapper.Map<Serie, SerieDto>(existente);
                }

                Logger.LogInformation("Serie not in DB, creating new entity...");
                
                // Mapeo Manual: Convierte datos de OMDB a la Entidad Serie
                var nuevaSerie = new Serie
                {
                    Title = datosExternos.Title ?? "Sin Título",
                    ImdbId = datosExternos.imdbID,
                    Genre = datosExternos.Genre ?? "Desconocido",

                    Director = datosExternos.Director != null && datosExternos.Director != "N/A"
                               ? datosExternos.Director
                               : "Desconocido",

                    Writer = datosExternos.Writer != null && datosExternos.Writer != "N/A"
                               ? datosExternos.Writer
                               : "Desconocido",

                    Language = datosExternos.Language != null && datosExternos.Language != "N/A"
                               ? datosExternos.Language
                               : "Original",

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
                                  : (datosExternos.Plot ?? "Sin descripción"),

                    TotalSeasons = int.TryParse(datosExternos.totalSeasons, out int seasons) ? seasons : 0
                };

                Logger.LogInformation("Created Serie entity: Title={Title}, ImdbId={ImdbId}", nuevaSerie.Title, nuevaSerie.ImdbId);

                // Obtiene información de temporadas desde OMDB
                if (nuevaSerie.TotalSeasons > 0)
                {
                    Logger.LogInformation("Fetching {TotalSeasons} seasons from OMDB...", nuevaSerie.TotalSeasons);
                    var fetchedSeasons = await FetchSeasonsAsync(nuevaSerie.ImdbId, nuevaSerie.TotalSeasons);
                    nuevaSerie.Seasons = fetchedSeasons;
                    Logger.LogInformation("Fetched {Count} seasons successfully", fetchedSeasons.Count);
                }
                else
                {
                    nuevaSerie.Seasons = new List<ScriptedReviews.Seasons.Season>();
                }

                // Guarda en la BDD
                Logger.LogInformation("Inserting serie into database...");
                var serieInsertada = await _serieRepository.InsertAsync(nuevaSerie, autoSave: true);
                Logger.LogInformation("Serie inserted with Id: {Id}", serieInsertada?.Id ?? -1);

                Logger.LogInformation("Mapping inserted serie to DTO...");
                var resultDto = ObjectMapper.Map<Serie, SerieDto>(serieInsertada);
                Logger.LogInformation("ObjectMapper.Map result: {Result}", resultDto == null ? "NULL" : $"OK (Id={resultDto.Id}, Title={resultDto.Title})");
                
                Logger.LogInformation("=== ImportarSerieAsync COMPLETED successfully ===");
                return resultDto;
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning("UserFriendlyException in ImportarSerieAsync: {Message}", ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "UNEXPECTED Exception in ImportarSerieAsync: {Message}", ex.Message);
                throw; 
            }
        }
    }
}