using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ratings.DTOs
{
    public class RatingDto
    {
        public int ProductId { get; set; }
        public int Stars { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
