using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Products.DTOs
{
    public record PhotoDTO
    {
        public string ImageName { get; init; } = string.Empty;
        public int ProductId { get; init; }
    }
}
