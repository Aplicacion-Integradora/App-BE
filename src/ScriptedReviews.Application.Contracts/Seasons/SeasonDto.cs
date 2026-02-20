using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Seasons
{
    public class SeasonDto : EntityDto<int>
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public string ReleaseDate { get; set; }

        public string Chapters { get; set; }
    }
}
