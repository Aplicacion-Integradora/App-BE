using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Ratings
{
    public class CreateRatingDto
    {
        public Guid SeriesId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class RatingDto
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public Guid SeriesId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
