using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        public CountryRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<int> MaxIdNumberAsync()
        {
            return await this._context.Countries.MaxAsync(c => c.IdNumber);
        }
    }
}