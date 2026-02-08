using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using ScriptedReviews.Series;
using ScriptedReviews.Users;

namespace ScriptedReviews.Ratings
{
    public class Rating : AggregateRoot<Guid>
    {
        public Guid UserId { get; set; } // ID del usuario que hizo la calificación
        public int SeriesId { get; set; } // ID de la serie calificada
        public int RatingNumber { get; set; } // Puntuación de 1 a 5
        public string Comment { get; set; } // Comentario opcional

        // Relación con la entidad Series
        public virtual Serie Serie { get; set; }

        // Relación con la entidad User
        //public virtual User User { get; set; }
    }
}
