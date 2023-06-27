using PhimMoi.Models.Movie;

namespace PhimMoi.Models
{
    public class HomeViewModel
    {
        public List<MovieViewModel>? ListRandomMovies { get; set; }
        public MovieViewModel[]? ListMovieNew { get; set; }
        public MovieViewModel[]? ListMovieTopRating { get; set; }
        public MovieViewModel[]? ListPhimLe { get; set; }
        public MovieViewModel[]? ListPhimBo { get; set; }
    }
}