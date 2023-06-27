using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(PhimMoiDbContext context) : base(context) { }
    }
}