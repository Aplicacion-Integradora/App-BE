using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ScriptedReviews.Chapters
{
    public class Chapter : AggregateRoot<int>
    {
        public int Number { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public required string Duration { get; set; }
        public required string Director { get; set; }
        public required string Writer { get; set; }

    }
}