using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Infrastructure.Context;
using PhimMoi.Infrastructure.Email;
using PhimMoi.Infrastructure.Identity;

namespace PhimMoi.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(setting =>
            {
                configuration.GetSection("MailSettings").Bind(setting);
            });
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddDbContext<PhimMoiDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PhimMoi") ?? ""));

            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<PhimMoiDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.AllowedUserNameCharacters = configuration["Characters"];
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
            });

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}