using Core.Entities.OrderAggregate;
using Core.Sharing.Pagination;
using Core.Sharing.Pagination.Core.Sharing;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(OrderParams orderParams);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(
            string buyerEmail,
            params Expression<Func<Order, object>>[] includes);
    }
}