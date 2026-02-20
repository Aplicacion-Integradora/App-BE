using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedReviews.Series
{
    public class CreateUpdateSerieDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Genre { get; set; }

        public string ReleaseDate { get; set; }
        public string Duration { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Cast { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
    }
}