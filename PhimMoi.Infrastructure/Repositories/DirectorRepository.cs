using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class DirectorRepository : Repository<Director>, IDirectorRepository
    {
        public DirectorRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<int> MaxIdNumberAsync()
        {
            return await this._context.Directors.MaxAsync(d => d.IdNumber);
        }
    }
}