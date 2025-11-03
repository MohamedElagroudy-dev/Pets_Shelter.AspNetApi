using Application.Common;
using Application.Common.Pagination;
using Application.Orders.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Orders.Services
{
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetAllAsync(OrderParams orderParams);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task<IReadOnlyList<OrderDto>> GetOrdersForUserAsync();
        Task<OrderDto> GetUserOrderByIdAsync(int orderId);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}