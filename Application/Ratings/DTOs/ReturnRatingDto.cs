using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ratings.DTOs
{
    public class ReturnRatingDto
    {
        public int Id { get; internal set; }
        public int Stars { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ReviewTime { get; set; }
    }
}
