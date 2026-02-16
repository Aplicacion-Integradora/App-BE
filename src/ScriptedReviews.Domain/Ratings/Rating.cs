using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using ScriptedReviews.Series;


namespace ScriptedReviews.Ratings
{
    public class Rating : AggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public int SeriesId { get; set; }
        public int RatingNumber { get; set; }
        public string Comment { get; set; }
        public virtual Serie Serie { get; set; }
    }
}
