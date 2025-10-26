using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Categories.DTOs
{
    public record UpdateCategoryDTO : AddCategoryDTO
    {
        public int Id { get; set; }
    }
}
