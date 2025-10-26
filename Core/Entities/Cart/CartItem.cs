using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Cart
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<string> PictureUrls { get; set; } = new();
    }
}
