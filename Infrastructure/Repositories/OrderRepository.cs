using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Sharing.Pagination;
using Core.Sharing.Pagination.Core.Sharing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(OrderParams orderParams)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.ItemOrdered)
                .Include(o => o.DeliveryMethod)
                .AsNoTracking();

            //  Filtering by BuyerEmail
            if (!string.IsNullOrEmpty(orderParams.BuyerEmail))
                query = query.Where(o => o.BuyerEmail == orderParams.BuyerEmail);

            //  Filtering by Status
            if (orderParams.Status.HasValue)
                query = query.Where(o => o.Status == orderParams.Status.Value);

            //  Searching (on email or shipping name)
            if (!string.IsNullOrEmpty(orderParams.Search))
            {
                string search = orderParams.Search.ToLower();
                query = query.Where(o =>
                    o.BuyerEmail.ToLower().Contains(search) ||
                    o.ShippingAddress.Name.ToLower().Contains(search)
                );
            }

            //  Get total before pagination
            int totalCount = await query.CountAsync();

            //  Sorting
            query = orderParams.Sort switch
            {
                OrderSort.DateAsc => query.OrderBy(o => o.OrderDate),
                OrderSort.DateDesc => query.OrderByDescending(o => o.OrderDate),
                OrderSort.PriceAsc => query.OrderBy(o => o.Subtotal),
                OrderSort.PriceDesc => query.OrderByDescending(o => o.Subtotal),
                _ => query.OrderByDescending(o => o.Id)
            };


            // 📄 Pagination
            query = query
                .Skip(orderParams.PageSize * (orderParams.PageNumber - 1))
                .Take(orderParams.PageSize);

            var orders = await query.ToListAsync();

            return (orders, totalCount);
        }


        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(
            string buyerEmail,
            params Expression<Func<Order, object>>[] includes)
        {
            IQueryable<Order> query = _context.Orders;

            foreach (var include in includes)
                query = query.Include(include);

            return await query
                .Where(o => o.BuyerEmail == buyerEmail)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
