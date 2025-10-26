using Core.Entities.Product;
using Core.Interfaces;
using Core.Sharing;
using Core.Sharing.Pagination;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(ProductParams productParams)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .AsNoTracking();

            // Filtering by search words
            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var searchWords = productParams.Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(p => searchWords.All(word =>
                    p.Name.ToLower().Contains(word.ToLower()) ||
                    p.Description.ToLower().Contains(word.ToLower())
                ));
            }

            // Filtering by category
            if (productParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productParams.CategoryId);

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            query = productParams.Sort switch
            {
                ProductSort.PriceAsc => query.OrderBy(p => p.Price),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.Price),
                ProductSort.NameAsc => query.OrderBy(p => p.Name),
                ProductSort.NameDesc => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.Id) // default Id
            };


            // Pagination
            query = query
                .Skip(productParams.PageSize * (productParams.PageNumber - 1))
                .Take(productParams.PageSize);

            var products = await query.ToListAsync();
            return (products, totalCount);
        }



        
    }
}
