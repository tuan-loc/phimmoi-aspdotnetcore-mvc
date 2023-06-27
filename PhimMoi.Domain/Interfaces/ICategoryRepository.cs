using PhimMoi.Domain.Models;

namespace PhimMoi.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> MaxIdNumberAsync();
    }
}