using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();

        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<T> GetByidAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByAsync(
            Expression<Func<T, bool>> criteria,
            params Expression<Func<T, object>>[] includes);

        Task<T> GetAsync(int id);
        Task AddAsync(T Entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(int id);

        Task UpdateAsync(int id, T Entity);
        Task<int> CountAsync();
    }
}
