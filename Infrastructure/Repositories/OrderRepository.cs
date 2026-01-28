using Core.Constants;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Sharing.Pagination;
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


        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? buyerEmail,
            OrderStatus? status,
            OrderSort sort)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.ItemOrdered)
                .Include(o => o.DeliveryMethod)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(buyerEmail)) 
                query = query.Where(o => o.BuyerEmail == buyerEmail); 

            
            if (status.HasValue) 
                query = query.Where(o => o.Status == status.Value); 

            if (!string.IsNullOrEmpty(search)) 
            {
                string searchLower = search.ToLower(); 
                query = query.Where(o =>
                    o.BuyerEmail.ToLower().Contains(searchLower) ||
                    o.ShippingAddress.Name.ToLower().Contains(searchLower)
                );
            }

            int totalCount = await query.CountAsync();

            query = sort switch 
            {
                OrderSort.DateAsc => query.OrderBy(o => o.OrderDate),
                OrderSort.DateDesc => query.OrderByDescending(o => o.OrderDate),
                OrderSort.PriceAsc => query.OrderBy(o => o.Subtotal),
                OrderSort.PriceDesc => query.OrderByDescending(o => o.Subtotal),
                _ => query.OrderByDescending(o => o.Id) // default 
            };


            // 📄 Pagination
            query = query
                .Skip(pageSize * (pageNumber - 1)) 
                .Take(pageSize); 

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
