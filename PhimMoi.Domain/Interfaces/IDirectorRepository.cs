using PhimMoi.Domain.Models;

namespace PhimMoi.Domain.Interfaces
{
    public interface IDirectorRepository : IRepository<Director>
    {
        Task<int> MaxIdNumberAsync();
    }
}