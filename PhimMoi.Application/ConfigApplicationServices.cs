using Microsoft.Extensions.DependencyInjection;
using PhimMoi.Application.Interfaces;
using PhimMoi.Application.Services;

namespace PhimMoi.Application
{
    public static class ConfigApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<ICastService, CastService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDirectorService, DirectorService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICommentService, CommentService>();

            return services;
        }
    }
}