using Core.Constants;
using Core.Entities.OrderAggregate;
using Core.Sharing.Pagination;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? buyerEmail,
            OrderStatus? status,
            OrderSort sort);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(
            string buyerEmail,
            params Expression<Func<Order, object>>[] includes);
    }
}