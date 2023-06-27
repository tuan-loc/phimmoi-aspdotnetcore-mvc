using PhimMoi.Domain.Models;

namespace PhimMoi.Domain.Interfaces
{
    public interface ICastRepository : IRepository<Cast>
    {
        Task<int> MaxIdNumberAsync();
    }
}