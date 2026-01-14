using ScriptedReviews.Series;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace ScriptedReviews.Watchlists
{
    public class Watchlist : AuditedAggregateRoot<int>
    {
       
        public Guid UserId { get; set; }

        public List<Serie> Series { get; set; }

        protected Watchlist() { }

        
        public Watchlist(Guid userId)
        {
            UserId = userId;
            Series = new List<Serie>();
        }
    }
}