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
        public string Year { get; set; }  // Fecha Lanzamiento
        public string Runtime { get; set; } // Duración
        public string Director { get; set; } // Parte del equipo
        public string Actors { get; set; }   // Parte del equipo
        public string Writer { get; set; }
        public string Poster { get; set; }   // Foto Portada
        public string Country { get; set; }
        public string imdbRating { get; set; } // Calificación IMDB
        public string Plot { get; set; } // Descripción
        public string Response { get; set; } // "True" o "False"
        public string Language { get; set; }
    }
}