using Application.Account;
using Application.Common;
using Application.Orders.DTOs;
using Application.Orders.Mappings;
using Core.Entities.OrderAggregate;
using Core.Exceptions;
using Core.Interfaces;
using Core.Sharing.Pagination;
using Core.Sharing.Pagination.Core.Sharing;
using Ecom.Application.Products.DTOs;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task<PagedResult<OrderDto>> GetAllAsync(OrderParams orderParams)
        {
            _logger.LogInformation("Executing GetAllAsync with page {PageNumber}, size {PageSize}", orderParams.PageNumber, orderParams.PageSize);
            var (orders, totalCount) = await _unitOfWork.Orders.GetAllAsync(orderParams);

            var data = orders.Select(o => o.ToDto()).ToList();

            return new PagedResult<OrderDto>(data, totalCount, orderParams.PageSize, orderParams.PageNumber);
        }
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order for cartId: {CartId}", dto.CartId);

            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var cart = await _unitOfWork.Cart.GetCartAsync(dto.CartId);
            if (cart == null)
                throw new ArgumentException("Cart not found");
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
                throw new InvalidOperationException("No payment intent for this order");

            var items = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var product = await _unitOfWork.Products.GetByidAsync(item.ProductId, p => p.Photos)
                              ?? throw new ArgumentException($"Product {item.ProductId} not found");

                var pictureUrl = product.Photos?.FirstOrDefault()?.ImageName ?? "default.jpg";

                items.Add(new OrderItem
                {
                    ItemOrdered = new ProductItemOrdered
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = pictureUrl
                    },
                    Price = product.Price,
                    Quantity = item.Quantity
                });
            }

            var deliveryMethod = await _unitOfWork.DeliveryMethods.GetAsync(dto.DeliveryMethodId)
                ?? throw new ArgumentException("Delivery method not found");

            var subtotal = items.Sum(x => x.Price * x.Quantity);

            var order = new Order
            {
                OrderItems = items,
                DeliveryMethod = deliveryMethod,
                ShippingAddress = dto.ShippingAddress,
                Subtotal = subtotal,
                PaymentSummary = dto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                BuyerEmail = user.Email
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return order.ToDto();
        }

        public async Task<IReadOnlyList<OrderDto>> GetOrdersForUserAsync()
        {
            _logger.LogInformation("Fetching orders for current user");

            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var orders = await _unitOfWork.Orders.GetOrdersForUserAsync(
                user.Email,
                o => o.DeliveryMethod,
                o => o.OrderItems
            );

            return orders.Select(o => o.ToDto()).ToList();
        }

        public async Task<OrderDto> GetUserOrderByIdAsync(int orderId)
        {
            _logger.LogInformation("Fetching order with id: {OrderId}", orderId);

            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var order = await _unitOfWork.Orders.GetByAsync(
                o => o.Id == orderId && o.BuyerEmail == user.Email,
                o => o.DeliveryMethod,
                o => o.OrderItems
            );

            if (order == null)
                throw new NotFoundException(nameof(Order), orderId.ToString());

            return order.ToDto();
        }
        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            _logger.LogInformation("Fetching order with id: {OrderId}", orderId);

            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var order = await _unitOfWork.Orders.GetByAsync(
                o => o.Id == orderId,
                o => o.DeliveryMethod,
                o => o.OrderItems
            );

            if (order == null)
                throw new NotFoundException(nameof(Order), orderId.ToString());

            return order.ToDto();
        }
        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _unitOfWork.Orders.GetByidAsync(orderId, o => o.OrderItems, o => o.DeliveryMethod);

            if (order == null)
                throw new KeyNotFoundException($"No order found with ID {orderId}.");

            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
                throw new InvalidOperationException($"Invalid order status: {newStatus}");

            // Example logic: Prevent invalid transitions
            if (order.Status == OrderStatus.Refunded)
                throw new InvalidOperationException("Cannot change the status of a refunded order.");

            if (order.Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Delivered orders cannot be changed.");

            order.Status = status;

            await _unitOfWork.CompleteAsync();

            return order.ToDto();
        }

    }
}
