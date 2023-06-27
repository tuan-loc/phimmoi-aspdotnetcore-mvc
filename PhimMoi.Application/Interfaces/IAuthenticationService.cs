using Microsoft.AspNetCore.Authentication;
using PhimMoi.Application.Models;

namespace PhimMoi.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResult> RegisterAsync(string name, string email, string password);
        Task<LoginResult> LoginAsync(string email, string password, bool rememberMe);
        Task<LoginResult> ExternalLoginAsync();
        Task LogoutAsync();
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
    }
}