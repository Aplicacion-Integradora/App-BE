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
        public required string Title { get; set; }
    }
}
