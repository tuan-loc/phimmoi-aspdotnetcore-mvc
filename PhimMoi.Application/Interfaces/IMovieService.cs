using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PhimMoi.Application.Interfaces
{
    public interface IMovieService
    {
        Task<PagedList<Movie>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Movie>> SearchAsync(MovieParameter movieParameter);
        Task<PagedList<Movie>> FindByTypeAsync(string type, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByYearAsync(int year, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindBeforeYearAsync(int year, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByTagAsync(string tag, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByCastIdAsync(string castId, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByCategoryIdAsync(string categoryId, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByDirectorIdAsync(string directorId, PagingParameter pagingParameter);
        Task<PagedList<Movie>> FindByCountryIdAsync(string countryId, PagingParameter pagingParameter);
        Task<PagedList<Movie>> GetTrailerAsync(PagingParameter pagingParameter);
        Task<PagedList<Movie>> GetMoviesOrderByRatingAsync(PagingParameter pagingParameter);
        Task<IEnumerable<Movie>> GetRandomMoviesAsync(int count);
        Task<IEnumerable<Movie>> GetRelateMoviesAsync(string movieId, int maxCount);
        Task<Movie?> GetByIdAsync(string id, Expression<Func<Movie, object?>>[]? includes = null);
        Task<Movie> CreateAsync(Movie movie, string[]? casts = null, string[]? directors = null, string[]? categories = null, string? country = null, string? tags = null, string[]? videos = null);
        Task<Movie> UpdateAsync(string movieId, Movie movie, string[]? casts = null, string[]? directors = null, string[]? categories = null, string? country = null, string? tags = null, string[]? videos = null);
        Task DeleteAsync(string movieId);
        Task IncreateViewAsync(string movieId);
        Task<bool> LikeMovieAsync(string movieId, string userId);
    }
}