using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;

namespace PhimMoi.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<PagedList<Movie>> GetAsync(MovieParameter movieParameter);
        Task<PagedList<Movie>> GetMovieByTagNameAsync(string tagName, PagingParameter pagingParameter);
        Task<int> MaxIdNumberAsync();
    }
}