using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.Product;
using Core.Interfaces;
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

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
            int pageNumber,             
            int pageSize,               
            string? search,             
            int? categoryId,         
            int? petTypeId,
            ProductSort? sort) 
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .Include(p => p.PetType)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                var searchWords = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(p => searchWords.All(word =>
                    p.Name.ToLower().Contains(word.ToLower()) ||
                    p.Description.ToLower().Contains(word.ToLower())
                ));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (petTypeId.HasValue)
                query = query.Where(p => p.PetTypeId == petTypeId);

            int totalCount = await query.CountAsync();

            query = sort switch
            {
                ProductSort.PriceAsc => query.OrderBy(p => p.Price),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.Price),
                ProductSort.NameAsc => query.OrderBy(p => p.Name),
                ProductSort.NameDesc => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.Id)
            };

            query = query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            var products = await query.ToListAsync();
            return (products, totalCount);
        }
   }
}
