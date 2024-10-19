using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using ScriptedReviews.Series;

namespace ScriptedReviews.Watchlists.Dtos
{
    public class WatchlistDto : EntityDto<int>
    {
        public List<SerieDto> Series { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasChanges { get; set; }
    }
}
