using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class VideoRepository : Repository<Video>
    {
        public VideoRepository(PhimMoiDbContext context) : base(context) { }
    }
}