using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Text.Json;

namespace ScriptedReviews.Series
{
    public class SerieAppService: CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {
        // API Key de OMDB 
        private const string OmdbApiKey = "35900e06";

        public SerieAppService(IRepository<Serie, int> repository) : base(repository)
        {
        }

        // --- OPERACIÓN 2.2 ---
        public async Task<SerieDto> ImportarSerieAsync(string titulo)
        {
            // 1. Verificar si ya existe la serie en la BD
            var existente = await Repository.FirstOrDefaultAsync(x => x.Title.Contains(titulo));
            if (existente != null)
            {
                return ObjectMapper.Map<Serie, SerieDto>(existente);
            }

            // 2. Conectar con OMDB
            string url = $"http://www.omdbapi.com/?t={titulo}&apikey={OmdbApiKey}";
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

            // 3. Mapeo Manual: Convertir datos de OMDB a tu Entidad 'Serie'
            var nuevaSerie = new Serie
            {
                Title = datosExternos.Title ?? "Sin Título",
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
            var serieInsertada = await Repository.InsertAsync(nuevaSerie, autoSave: true);

            return ObjectMapper.Map<Serie, SerieDto>(serieInsertada);
        }
    }
}