using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedReviews.Series
{
    public class CreateUpdateSerieDto
    {
        [Required]
        public string Title { get; set; } // Título

        [Required]
        public string Genre { get; set; } // Género

        public string ReleaseDate { get; set; } // Fecha de lanzamiento

        public string Duration { get; set; } // Duración

        // Equipo (Director, Escritor, Elenco) 
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Cast { get; set; }

        public string Image { get; set; } // Foto de portada

        public string Country { get; set; } // País de origen

        public string Rating { get; set; } // Calificación en IMDB

        // Campo extra -- Opcional
        public string Description { get; set; }
    }
}