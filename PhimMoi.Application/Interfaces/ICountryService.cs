using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;

namespace PhimMoi.Application.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<PagedList<Country>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Country>> SearchAsync(string? value, PagingParameter pagingParameter);
        Task<Country?> GetByIdAsync(string id);
        Task<Country?> GetByNameAsync(string name);
        Task<Country> CreateAsync(Country country);
        Task<Country> UpdateAsync(string countryId, Country country);
        Task DeleteAsync(string countryId);
    }
}