using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Exceptions;
using PhimMoi.Domain.Exceptions.NotFound;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using System.Linq.Expressions;

namespace PhimMoi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Comment comment, string? userId = null, string? movieId = null, int responseToId = 0)
        {
            if(userId != null)
            {
                User? user = await _userService.FindByIdAsync(userId);
                if(user == null)
                {
                    throw new UserNotFoundException(userId);
                }

                comment.User = user;
            }

            if(movieId != null)
            {
                Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieId);
                if(movie == null)
                {
                    throw new MovieNotFoundException(movieId);
                }

                comment.Movie = movie;
            }

            if(responseToId > 0)
            {
                Comment? responseToComment = await GetByIdAsync(responseToId);
                comment.ResponseTo = responseToComment;
            }

            if(comment.Movie == null || comment.User == null)
            {
                throw new CommentNullException();
            }

            _unitOfWork.CommentRepository.Create(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int commentId)
        {
            Comment? comment = await _unitOfWork.CommentRepository.FirstOrDefaultAsync(c => c.Id == commentId, new Expression<Func<Comment, object?>>[]
            {
                c => c.Responses
            });
            if (comment == null) throw new Exception();

            if(comment.Responses != null && comment.Responses.Count > 0)
            {
                foreach(var cmt in comment.Responses)
                {
                    _unitOfWork.CommentRepository.Delete(cmt);
                }
            }

            _unitOfWork.CommentRepository.Delete(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CommentRepository.FirstOrDefaultAsync(c => c.Id == id, new Expression<Func<Comment, object?>>[]
            {
                c => c.User
            });
        }

        public async Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter)
        {
            return await _unitOfWork.CommentRepository.GetByMovieIdAsync(movieId, pagingParameter);
        }

        public async Task LikeComment(int commentId)
        {
            Comment? comment = await _unitOfWork.CommentRepository.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null) throw new Exception();
            comment.Like++;
            _unitOfWork.CommentRepository.Update(comment);
            await _unitOfWork.SaveAsync();
        }
    }
}