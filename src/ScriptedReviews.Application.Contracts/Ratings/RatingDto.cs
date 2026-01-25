using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Ratings
{
    public class RatingDto : EntityDto<int>
    {
        public Guid UserId { get; set; }
        public int SeriesId { get; set; }
        public int RatingNumber { get; set; } 
        public string Comment { get; set; }
    }
}