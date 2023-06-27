using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using PhimMoi.Mapper;
using PhimMoi.Infrastructure;
using PhimMoi.Application;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using PhimMoi.Transformer;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("policy1", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddRouting(option =>
{
    option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
});

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
                                 new SlugifyParameterTransformer()));
}).AddNewtonsoftJson();

builder.Services.AddAutoMapper(typeof(DomainToViewModelProfile), typeof(ViewModelToDomainProfile));

builder.Services.AddOptions();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddAuthentication()
    .AddFacebook(facebookOptions => {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.CallbackPath = "/dang-nhap-facebook";
        facebookOptions.Fields.Add("picture");
        facebookOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = (context) =>
            {
                ClaimsIdentity? identity = context.Principal != null ? (ClaimsIdentity?)context.Principal.Identity : null;
                string profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                if (identity != null) identity.AddClaim(new Claim("image", profileImg));
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.CallbackPath = "/login-google";
        googleOptions.ClaimActions.MapJsonKey("image", "picture");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var defaultDateCulture = "fr-FR";
var ci = new CultureInfo(defaultDateCulture);
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo>
    {
        ci,
    },
    SupportedUICultures = new List<CultureInfo>
    {
        ci,
    }
});

app.UseStatusCodePagesWithReExecute("/home/handle-error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("policy1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:slugify}/{controller:slugify}/{action:slugify}/{id:slugify?}",
        defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
        name: "default",
        pattern: "{controller:slugify}/{action:slugify}/{id:slugify?}",
        defaults: new { controller = "Home", action = "Index" });

app.Run();