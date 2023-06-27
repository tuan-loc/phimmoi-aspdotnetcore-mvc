using PhimMoi.Domain.PagingModel;
using PhimMoi.Models.Movie;

namespace PhimMoi.Models.Cast
{
    public class CastViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? About { get; set; }
        public PagedList<MovieViewModel> Movies { get; set; }

        public CastViewModel(string id, string name, PagedList<MovieViewModel> movies)
        {
            Id = id;
            Name = name;
            Movies = movies;
        }

    }
}