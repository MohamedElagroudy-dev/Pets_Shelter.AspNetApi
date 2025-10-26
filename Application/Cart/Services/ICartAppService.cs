using Core.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cart.Services
{
    public interface ICartAppService
    {
        Task<ShoppingCart?> GetCartByIdAsync(string id);
        Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart);
        Task<bool> DeleteCartAsync(string id);
    }
}

