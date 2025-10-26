using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecom.Application.Products.DTOs
{
    public record AddProductDTO
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }

        public int QuantityInStock { get; set; }
        public int CategoryId { get; init; }
        public required IFormFileCollection Photos { get; init; }
    }
}
