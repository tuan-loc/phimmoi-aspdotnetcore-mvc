using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class CastRepository : Repository<Cast>, ICastRepository
    {
        public CastRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<int> MaxIdNumberAsync()
        {
            return await this._context.Casts.MaxAsync(c => c.IdNumber);
        }
    }
}