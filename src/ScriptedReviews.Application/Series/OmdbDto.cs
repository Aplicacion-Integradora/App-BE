using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedReviews.Series
{
    // Esta clase solo sirve para recibir los datos de OMDB
    public class OmdbDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }
        public string Runtime { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Writer { get; set; }
        public string Poster { get; set; }
        public string Country { get; set; }
        public string imdbRating { get; set; }
        public string Plot { get; set; }
        public string Response { get; set; }
        public string Language { get; set; }
        public string imdbID { get; set; }
        public string totalSeasons { get; set; }
    }
}