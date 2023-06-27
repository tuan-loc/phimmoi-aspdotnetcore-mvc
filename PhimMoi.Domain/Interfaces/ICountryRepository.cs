using PhimMoi.Domain.Models;

namespace PhimMoi.Domain.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<int> MaxIdNumberAsync();
    }
}