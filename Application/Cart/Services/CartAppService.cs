using Core.Entities.Cart;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cart.Services
{
    public class CartAppService : ICartAppService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartAppService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Cart ID is required");

            return await _unitOfWork.Cart.GetCartAsync(id)
                   ?? new ShoppingCart { Id = id };
        }

        public async Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrWhiteSpace(cart.Id))
                throw new ArgumentException("Cart data is invalid");

            var updatedCart = await _unitOfWork.Cart.SetCartAsync(cart);

            if (updatedCart == null)
                throw new InvalidOperationException("Problem saving the cart");

            return updatedCart;
        }

        public async Task<bool> DeleteCartAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Cart ID is required");

            return await _unitOfWork.Cart.DeleteCartAsync(id);
        }
    }
}
