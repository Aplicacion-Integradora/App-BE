using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ScriptedReviews.Seasons
{
    public class Season: AggregateRoot<int>
    {
        public int Number { get; set; }

        public required string Description { get; set; }

        public required string ReleaseDate { get; set; }

        public required string Chapters { get; set; }


    }
}
