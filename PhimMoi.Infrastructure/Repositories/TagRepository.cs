using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class TagRepository : Repository<Tag>
    {
        public TagRepository(PhimMoiDbContext context) : base(context) { }
    }
}