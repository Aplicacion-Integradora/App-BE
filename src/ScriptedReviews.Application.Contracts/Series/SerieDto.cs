using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Series
{
    public class SerieDto: EntityDto<int>
    {
        public  string Title { get; set; }
        public  string Description { get; set; }
        public  string Image { get; set; }
        public  string Genre { get; set; }
        public  string Language { get; set; }
        public  string ReleaseDate { get; set; }
        public  string Duration { get; set; }
        public  string Rating { get; set; }
        public  string Country { get; set; }
        public  string Director { get; set; }
        public  string Cast { get; set; }
        public  string Writer { get; set; }

        // public  List<Season> Seasons { get; set; }

    }
}
