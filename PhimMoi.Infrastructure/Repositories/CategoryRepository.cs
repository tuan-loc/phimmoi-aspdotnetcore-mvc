using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<int> MaxIdNumberAsync()
        {
            return await this._context.Categories.MaxAsync(c => c.IdNumber);
        }
    }
}