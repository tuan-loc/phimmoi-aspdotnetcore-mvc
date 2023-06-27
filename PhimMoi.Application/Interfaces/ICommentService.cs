using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;

namespace PhimMoi.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter);
        Task CreateAsync(Comment comment, string? userId = null, string? movieId = null, int responseToId = 0);
        Task DeleteAsync(int commentId);
        Task LikeComment(int commentId);
    }
}