using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedReviews.Ratings
{
    public class UpdateRatingDto
    {
        [Required]
        public int Rating { get; set; } // La nueva puntuación 

        public string Comment { get; set; } // El nuevo comentario
    }
}