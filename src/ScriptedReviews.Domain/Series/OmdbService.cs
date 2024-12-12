using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ScriptedReviews.Series
{
    public class OmdbService : ISeriesApiService
    {
        private static readonly string apiKey = "a6754de0"; 
        private static readonly string baseUrl = "http://www.omdbapi.com/";

        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string gender)
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
                    series.Add(new SerieDto { Title = serieOmdb.Title });
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

            public string Rating { get; set; }

            public string Country { get; set; }

            public string Director { get; set; }

            public string Cast { get; set; }

            public string Writer { get; set; }
        }
    }
}