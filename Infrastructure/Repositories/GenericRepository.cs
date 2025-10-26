using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T Entity)
        {
            await _context.Set<T>().AddAsync(Entity);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }



        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(entity);
        }

        public IEnumerable<T> GetAll()

         => _context.Set<T>().AsNoTracking().ToList();


        public async Task<IReadOnlyList<T>> GetAllAsync()
        => await _context.Set<T>().AsNoTracking().ToListAsync();
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                query = query.Include(item);

            }
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(predicate).AsNoTracking().ToListAsync();
        }


        public async Task<T> GetAsync(int id)
         => await _context.Set<T>().FindAsync(id);

        public async Task<T> GetByidAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            //  IQueryable<T> query = _context.Set<T>();
            IQueryable<T> query = _context.Set<T>().Where(x => x.Id == id);
            foreach (var item in includes)
            {
                query = query.Include(item);

            }
            return await query.FirstOrDefaultAsync();
            //  return await ((DbSet<T>)query).FindAsync();

        }
        public async Task<T?> GetByAsync(
            Expression<Func<T, bool>> criteria,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply where criteria
            query = query.Where(criteria);

            return await query.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(int id, T Entity)
        {
            var ex_entity = await _context.Set<T>().FindAsync(id);
            if (ex_entity != null)
            {
                _context.Update(ex_entity);
            }
        }

        public async Task<int> CountAsync()
         => await _context.Set<T>().CountAsync();
    }
}
