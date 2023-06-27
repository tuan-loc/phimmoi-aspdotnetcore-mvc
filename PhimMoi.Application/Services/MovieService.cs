using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Exceptions.NotFound;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.SharedLibrary.Helpers;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace PhimMoi.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Movie> CreateAsync(Movie movie, string[]? casts = null, string[]? directors = null, string[]? categories = null, string? country = null, string? tags = null, string[]? videos = null)
        {
            movie.IdNumber = await _unitOfWork.MovieRepository.AnyAsync() ? await _unitOfWork.MovieRepository.MaxIdNumberAsync() + 1 : 1;
            movie.Id = "ps" + movie.IdNumber.ToString();
            movie.Name = movie.Name.NormalizeString();
            movie.NormalizeName = movie.Name.RemoveMarks();
            movie.TranslateName = movie.TranslateName.NormalizeString();
            movie.NormalizeTranslateName = movie.TranslateName.RemoveMarks();
            movie.CreatedDate = DateTime.Now;

            if(categories != null && categories.Length > 0)
            {
                foreach (string cate in categories)
                {
                    Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Id == cate);
                    if (category != null) continue;
                    movie.Categories ??= new List<Category>();
                    movie.Categories.Add(category);
                }
            }

            if(casts != null && casts.Length > 0)
            {
                foreach (string castName in casts)
                {
                    Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Id == castName);
                    if (cast == null) continue;
                    movie.Casts ??= new List<Cast>();
                    movie.Casts.Add(cast);
                }
            }

            if(directors != null && directors.Length > 0)
            {
                foreach (string directorName in directors)
                {
                    Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Id == directorName);
                    if (director == null) continue;
                    movie.Directors ??= new List<Director>();
                    movie.Directors.Add(director);
                }
            }

            if (!string.IsNullOrEmpty(country))
            {
                Country? countrytoAdd = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == country);
                if(countrytoAdd != null)
                {
                    movie.Country = countrytoAdd;
                }
            }

            if(videos != null && videos.Length > 0)
            {
                int index = 0;
                foreach (var v in videos)
                {
                    index += 1;
                    if (string.IsNullOrEmpty(v)) continue;

                    Video video = new()
                    {
                        VideoUrl = v,
                        Episode = index,
                        Movie = movie
                    };

                    movie.Videos ??= new List<Video>();
                    movie.Videos.Add(video);
                }
            }

            if(tags != null)
            {
                string[] tagNames = new string[0];
                if(!string.IsNullOrEmpty(tags) && !string.IsNullOrWhiteSpace(tags))
                {
                    tagNames = new Regex(@", |,").Split(tags);
                }

                foreach (var tagName in tagNames)
                {
                    if (string.IsNullOrEmpty(tagName) || string.IsNullOrWhiteSpace(tagName)) continue;
                    Tag tag = new()
                    {
                        TagName = tagName.Trim(),
                        Movie = movie
                    };

                    movie.Tags ??= new List<Tag>();
                    movie.Tags.Add(tag);
                }
            }

            _unitOfWork.MovieRepository.Create(movie);
            await _unitOfWork.SaveAsync();
            return movie;
        }

        public async Task DeleteAsync(string movieId)
        {
            Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieId);
            if (movie == null) throw new MovieNotFoundException(movieId);
            _unitOfWork.MovieRepository.Delete(movie);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PagedList<Movie>> GetAllAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, orderBy: movies => movies.OrderByDescending(mv => mv.CreatedDate));
        }

        public async Task<Movie?> GetByIdAsync(string id, Expression<Func<Movie, object?>>[]? includes = null)
        {
            return await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == id, includes);
        }

        public async Task<Movie> UpdateAsync(string movieId, Movie movie, string[]? casts = null, string[]? directors = null, string[]? categories = null, string? country = null, string? tags = null, string[]? videos = null)
        {
            Movie? movieToEdit = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieId, new Expression<Func<Movie, object?>>[]
            {
                m => m.Casts,
                m => m.Categories,
                m => m.Directors,
                m => m.Country,
                m => m.Tags,
                m => m.Videos,
            });

            if (movieToEdit == null) throw new MovieNotFoundException(movieId);

            movieToEdit.Name = movie.Name.NormalizeString();
            movieToEdit.NormalizeName = movieToEdit.Name.RemoveMarks();
            movieToEdit.TranslateName = movie.TranslateName.NormalizeString();
            movieToEdit.NormalizeTranslateName = movie.TranslateName.RemoveMarks();
            movieToEdit.Description = movie.Description;
            movieToEdit.ReleaseDate = movie.ReleaseDate;
            movieToEdit.Length = movie.Length;
            movieToEdit.Trailer = movie.Trailer;
            movieToEdit.Rating = movie.Rating;
            movieToEdit.Type = movie.Type;
            movieToEdit.Status = movie.Status;
            movieToEdit.EpisodeCount = movie.EpisodeCount;
            movieToEdit.Image = movie.Image;
            movieToEdit.CreatedDate = DateTime.Now;

            if(movieToEdit.Country != null)
            {
                if(!string.IsNullOrEmpty(country) && country != movieToEdit.Country.Name)
                {
                    Country? countryToEdit = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == country);

                    if(countryToEdit != null)
                    {
                        movieToEdit.Country = countryToEdit;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(country))
                {
                    Country? countryToEdit = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == country);

                    if(countryToEdit != null)
                    {
                        movieToEdit.Country = countryToEdit;
                    }
                }
            }

            if(categories != null && categories.Length > 0)
            {
                if(movieToEdit.Categories != null)
                {
                    movieToEdit.Categories = null;
                }

                foreach (string cate in categories)
                {
                    Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Id == cate);
                    if (category == null) continue;
                    movieToEdit.Categories ??= new List<Category>();
                    movieToEdit.Categories.Add(category);
                }
            }

            if(casts != null && casts.Length > 0)
            {
                if(movieToEdit.Casts != null)
                {
                    movieToEdit.Casts = null;
                }

                foreach (string castName in casts)
                {
                    Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Id == castName);
                    if (cast == null) continue;
                    movieToEdit.Casts ??= new List<Cast>();
                    movieToEdit.Casts.Add(cast);
                }
            }

            if(directors != null && directors.Length > 0)
            {
                if(movieToEdit.Directors != null)
                {
                    movieToEdit.Directors = null;
                }

                foreach (string directorName in directors)
                {
                    Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Id == directorName);
                    if (director == null) continue;
                    movieToEdit.Directors ??= new List<Director>();
                    movieToEdit.Directors.Add(director);
                }
            }

            if(videos != null && videos.Length > 0)
            {
                int index = 0;
                if (movieToEdit.Videos != null) movieToEdit.Videos = null;
                foreach (var v in videos)
                {
                    index += 1;
                    if (string.IsNullOrEmpty(v)) continue;
                    Video video = new()
                    {
                        VideoUrl = v,
                        Episode = index,
                        Movie = movieToEdit
                    };

                    movieToEdit.Videos ??= new List<Video>();
                    movieToEdit.Videos.Add(video);
                }
            }

            if(tags != null)
            {
                string[] tagNames = new string[0];
                if(!string.IsNullOrEmpty(tags) && !string.IsNullOrWhiteSpace(tags))
                {
                    tagNames = new Regex(@", |,").Split(tags);
                }

                if (movieToEdit.Tags != null) movieToEdit.Tags = null;

                foreach (string tagName in tagNames)
                {
                    if (string.IsNullOrEmpty(tagName) || string.IsNullOrWhiteSpace(tagName)) continue;

                    Tag tag = new()
                    {
                        TagName = tagName,
                        Movie = movieToEdit
                    };

                    movieToEdit.Tags ??= new List<Tag>();
                    movieToEdit.Tags.Add(tag);
                }
            }

            _unitOfWork.MovieRepository.Update(movieToEdit);
            await _unitOfWork.SaveAsync();
            return movieToEdit;
        }

        public async Task<PagedList<Movie>> SearchAsync(MovieParameter movieParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(movieParameter);
        }

        public async Task<PagedList<Movie>> FindByTypeAsync(string type, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Type == type, movies => movies.OrderByDescending(m => m.CreatedDate));
        }

        public async Task<PagedList<Movie>> FindByYearAsync(int year, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.ReleaseDate != null && m.ReleaseDate.Value.Year == year, movies => movies.OrderByDescending(m => m.ReleaseDate));
        }

        public async Task<PagedList<Movie>> FindBeforeYearAsync(int year, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.ReleaseDate != null && m.ReleaseDate.Value.Year <= year, movies => movies.OrderByDescending(m => m.ReleaseDate));
        }

        public async Task<PagedList<Movie>> FindByTagAsync(string tag, PagingParameter pagingParameter)
        {
            tag = tag.ToLower().Trim();
            return await _unitOfWork.MovieRepository.GetMovieByTagNameAsync(tag, pagingParameter);
        }

        public async Task<PagedList<Movie>> GetTrailerAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Status == "Trailer", movies => movies.OrderByDescending(m => m.ReleaseDate));
        }

        public async Task<PagedList<Movie>> GetMoviesOrderByRatingAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, orderBy: movies => movies.OrderByDescending(m => m.Rating));
        }

        public async Task<IEnumerable<Movie>> GetRelateMoviesAsync(string movieId, int maxCount)
        {
            if (maxCount <= 0) maxCount = 1;

            Movie? movie = await this.GetByIdAsync(movieId, new Expression<Func<Movie, object?>>[]
            {
                m => m.Tags,
                m => m.Categories,
            });

            if(movie == null)
            {
                return new List<Movie>();
            }

            List<Movie> movies = new();
            List<Movie> movieList = (await _unitOfWork.MovieRepository.GetAsync(includes: new Expression<Func<Movie, object?>>[]
            {
                m => m.Categories
            })).ToList();

            if(movie.Tags != null && movie.Tags.Any())
            {
                string tagToFind = movie.Tags[0].TagName.ToLower();

                movies.AddRange((await _unitOfWork.TagRepository.GetAsync(new PagingParameter(1, maxCount, false), t => t.TagName.ToLower() == tagToFind && t.Movie.Id != movieId, includes: new Expression<Func<Tag, object?>>[]
                {
                    t => t.Movie
                })).Select(t => t.Movie).ToList());
            }

            if(movies.Count < maxCount && movie.Categories != null && movie.Categories.Any())
            {
                Category? cateToFind = movie.Categories[0];
                movies.AddRange(movieList.Where(m => !movies.Contains(m) && !m.Equals(movie) && m.Categories != null && m.Categories.Any(c => c.Equals(cateToFind))).Take(maxCount - movies.Count).ToList());
            }

            if(movies.Count < maxCount)
            {
                Random random = new();
                int ran;
                int count = movieList.Count;
                while(movies.Count < maxCount)
                {
                    if (movies.Count >= count) break;
                    ran = random.Next(0, count);
                    if (!movies.Contains(movieList[ran]) && !movieList[ran].Equals(movie))
                    {
                        movies.Add(movieList[ran]);
                    }
                }
            }

            return movies;
        }

        public async Task<IEnumerable<Movie>> GetRandomMoviesAsync(int count)
        {
            PagingParameter pagingParameter = new(1, count, false);
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, orderBy: movies => movies.OrderBy(mv => Guid.NewGuid()));
        }

        public async Task IncreateViewAsync(string movieId)
        {
            Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieId);
            if (movie == null) return;
            movie.View++;
            _unitOfWork.MovieRepository.Update(movie);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> LikeMovieAsync(string movieId, string userId)
        {
            Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieId, new Expression<Func<Movie, object?>>[]
            {
                m => m.LikedUsers
            });

            if(movie == null)
            {
                throw new MovieNotFoundException(movieId);
            }

            User? user = await _userService.FindByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFoundException(userId);
            }

            movie.LikedUsers ??= new List<User>();
            if (movie.LikedUsers.Contains(user))
            {
                movie.LikedUsers.Remove(user);
                _unitOfWork.MovieRepository.Update(movie);
                await _unitOfWork.SaveAsync();
                return false;
            }
            else
            {
                movie.LikedUsers.Add(user);
                _unitOfWork.MovieRepository.Update(movie);
                await _unitOfWork.SaveAsync();
                return true;
            }
        }

        public async Task<PagedList<Movie>> FindByCastIdAsync(string castId, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Casts.Any(c => c.Id == castId));
        }

        public async Task<PagedList<Movie>> FindByCategoryIdAsync(string categoryId, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Categories.Any(c => c.Id == categoryId));
        }

        public async Task<PagedList<Movie>> FindByDirectorIdAsync(string directorId, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Directors.Any(d => d.Id == directorId));
        }

        public async Task<PagedList<Movie>> FindByCountryIdAsync(string countryId, PagingParameter pagingParameter)
        {
            return await _unitOfWork.MovieRepository.GetAsync(pagingParameter, m => m.Country.Id == countryId);
        }
   }
}