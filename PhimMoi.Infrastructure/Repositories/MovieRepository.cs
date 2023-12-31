﻿using Microsoft.EntityFrameworkCore;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Infrastructure.Context;
using PhimMoi.SharedLibrary.Helpers;

namespace PhimMoi.Infrastructure.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(PhimMoiDbContext context) : base(context) { }

        public async Task<PagedList<Movie>> GetMovieByTagNameAsync(string tagName, PagingParameter pagingParameter)
        {
            IQueryable<Movie> movies = this._context.Tags.Where(t => t.TagName.ToLower() == tagName).Include(t => t.Movie).Select(t => t.Movie);
            return await PagedList<Movie>.ToPagedListAsync(movies, pagingParameter.Page, pagingParameter.Size, pagingParameter.AllowCalculateCount);
        }

        public async Task<int> MaxIdNumberAsync()
        {
            return await this._context.Movies.MaxAsync(m => m.IdNumber);
        }

        public async Task<PagedList<Movie>> GetAsync(MovieParameter movieParameter)
        {
            movieParameter.Value = movieParameter.Value?.RemoveMarks();
            IQueryable<Movie> movies = _context.Movies;
            if (!string.IsNullOrEmpty(movieParameter.Value))
            {
                movies = movies.Where(m => (m.NormalizeTranslateName ?? "").Contains(movieParameter.Value) || (m.NormalizeName ?? "").Contains(movieParameter.Value));
            }

            if (movieParameter.Year > 0)
            {
                movies = movies.Where(m => m.ReleaseDate != null && m.ReleaseDate.Value.Year == movieParameter.Year);
            }else if(movieParameter.BeforeYear > 0)
            {
                movies = movies.Where(m => m.ReleaseDate != null && m.ReleaseDate.Value.Year <= movieParameter.BeforeYear);
            }

            if (!string.IsNullOrEmpty(movieParameter.Type))
            {
                if(movieParameter.Type.ToLower() == "phimle")
                {
                    movies = movies.Where(m => m.Type == "Phim lẻ");
                }else if(movieParameter.Type.ToLower() == "phimbo")
                {
                    movies = movies.Where(m => m.Type == "Phim bộ");
                }
            }

            if (!string.IsNullOrEmpty(movieParameter.Status))
            {
                if(movieParameter.Status.ToLower() == "fullhd")
                {
                    movies = movies.Where(m => m.Status == "Full HD");
                }else if(movieParameter.Status.ToLower() == "trailer")
                {
                    movies = movies.Where(m => m.Status == "Trailer");
                }else if(movieParameter.Status.ToLower() == "cam")
                {
                    movies = movies.Where(m => m.Status == "CAM");
                }
            }

            if (!string.IsNullOrEmpty(movieParameter.Tag))
            {
                movieParameter.Tag = movieParameter.Tag.ToLower().Trim();
                movies = movies.Where(m => m.Tags.Any(t => t.TagName.ToLower() == movieParameter.Tag));
            }

            if (!string.IsNullOrEmpty(movieParameter.Country))
            {
                movieParameter.Country = movieParameter.Country.ToLower().Trim();
                movies = movies.Where(m => m.Country.Id == movieParameter.Country);
            }

            if(movieParameter.Categories != null && movieParameter.Categories.Length > 0)
            {
                movies = movies.Include(m => m.Categories).Where(m => m.Categories.Select(c => c.Id).Where(c => movieParameter.Categories.Contains(c)).Count() == movieParameter.Categories.Length);
            }

            if (!string.IsNullOrEmpty(movieParameter.OrderBy))
            {
                if (movieParameter.OrderBy.EndsWith("_desc"))
                {
                    movieParameter.OrderBy = movieParameter.OrderBy[..^5];
                    var propertyInfo = typeof(Movie).GetProperties().FirstOrDefault(p => p.Name.Equals(movieParameter.OrderBy, StringComparison.InvariantCultureIgnoreCase));
                    if(propertyInfo != null)
                    {
                        movies = movies.OrderByDescending(m => EF.Property<object>(m, propertyInfo.Name));
                    }
                }
                else
                {
                    var propertyInfo = typeof(Movie).GetProperties().FirstOrDefault(p => p.Name.Equals(movieParameter.OrderBy, StringComparison.InvariantCultureIgnoreCase));
                    if(propertyInfo != null)
                    {
                        movies = movies.OrderBy(m => EF.Property<object>(m, propertyInfo.Name));
                    }
                }
            }

            return await PagedList<Movie>.ToPagedListAsync(movies, movieParameter.Page, movieParameter.Size, movieParameter.AllowCalculateCount);
        }
    }
}
