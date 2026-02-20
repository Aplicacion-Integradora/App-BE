using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Ratings
{
    public class CreateRatingDto
    {
        public int SeriesId { get; set; }
        
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")] 
        public int RatingNumber { get; set; }
        public string Comment { get; set; }
    }

    public class RatingDto
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public int SeriesId { get; set; }
        public int RatingNumber { get; set; }
        public string Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class UpdateRatingDto
    {
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int? RatingNumber { get; set; } // La nueva puntuación 

        public string Comment { get; set; } // El nuevo comentario
    }
}
