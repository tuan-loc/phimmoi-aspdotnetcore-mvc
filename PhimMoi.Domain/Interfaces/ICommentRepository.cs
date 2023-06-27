using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;

namespace PhimMoi.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter);
    }
}