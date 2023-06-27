using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;

namespace PhimMoi.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<PagedList<Category>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Category>> SearchAsync(string? value, PagingParameter pagingParameter);
        Task<Category?> GetByIdAsync(string id);
        Task<Category?> GetByNameAsync(string name);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(string categoryId, Category category);
        Task DeleteAsync(string categoryId);
    }
}