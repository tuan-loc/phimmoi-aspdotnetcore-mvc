using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using System.Linq.Expressions;

namespace PhimMoi.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, object?>>[]? includes = null
        );

        Task<PagedList<T>> GetAsync(
            PagingParameter pagingParameter,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, object?>>[]? includes = null
        );

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, object?>>[]? includes = null
        );

        T Create(T entity);

        T Update(T entity);

        void Delete(T entity);

        Task<bool> AnyAsync();
    }
}