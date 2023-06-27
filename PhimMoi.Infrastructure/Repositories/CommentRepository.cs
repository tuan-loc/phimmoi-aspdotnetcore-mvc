using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Infrastructure.Context;

namespace PhimMoi.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter)
        {
            IQueryable<Comment> comments = this._context.Comments
                .Include(c => c.Movie)
                .Include(c => c.User)
                .Include(c => c.ResponseTo)
                .Include(c => c.Responses)
                .ThenInclude(c => c.User)
                .Where(c => c.Movie.Id == movieId && c.ResponseTo == null)
                .OrderByDescending(c => c.CreatedAt);

            return await PagedList<Comment>.ToPagedListAsync(comments, pagingParameter.Page, pagingParameter.Size, pagingParameter.AllowCalculateCount);
        }
    }
}