using Microsoft.AspNetCore.Identity;
using PhimMoi.Application.Interfaces;
using PhimMoi.Application.Models;
using PhimMoi.Domain.Models;
using PhimMoi.SharedLibrary.Constants;
using PhimMoi.SharedLibrary.Helpers;
using System.Security.Claims;

namespace PhimMoi.Infrastructure.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthenticationService(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public Microsoft.AspNetCore.Authentication.AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }

        public async Task<LoginResult> ExternalLoginAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                return LoginResult.Error("Error loading external login information.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            LoginResult loginResult = LoginResult.FromSignResult(result);
            if(!loginResult.Success && !loginResult.IsLockedOut)
            {
                User? emailUser = null;
                if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    emailUser = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                }

                if(emailUser != null)
                {
                    if (!emailUser.EmailConfirmed)
                    {
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(emailUser);
                        await _userManager.ConfirmEmailAsync(emailUser, code);
                    }

                    if((await _userManager.AddLoginAsync(emailUser, info)).Succeeded)
                    {
                        await _signInManager.SignInAsync(emailUser, false);
                        loginResult = LoginResult.OK();
                    }
                }
                else
                {
                    var user = new User();
                    user.EmailConfirmed = true;

                    if(info.Principal.HasClaim(c => c.Type == "image"))
                    {
                        user.Avatar = info.Principal.FindFirstValue("image");
                    }

                    if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        user.DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name);
                        user.NormalizeDisplayName = user.DisplayName.RemoveMarks();
                    }

                    if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    {
                        user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                        user.UserName = user.Email;
                    }

                    var result2 = await _userManager.CreateAsync(user);
                    if (result2.Succeeded)
                    {
                        result2 = await _userManager.AddLoginAsync(user, info);
                        if (result2.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                            loginResult = LoginResult.OK();
                        }
                    }
                }
            }

            return loginResult;
        }

        public async Task<IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return await _signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<LoginResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new LoginResult { Success = false };
            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
            return LoginResult.FromSignResult(result);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResult> RegisterAsync(string name, string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null) return RegisterResult.Error("Email đã tồn tại.");
            user = new User()
            {
                UserName = email,
                Email = email,
                DisplayName = name,
                RoleName = RoleConstant.MEMBER
            };

            user.NormalizeDisplayName = user.DisplayName.RemoveMarks();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleConstant.MEMBER);
                await _signInManager.SignInAsync(user, false);
            }

            var registerResult = RegisterResult.ToAppResult(result);
            if (registerResult.Success) registerResult.User = user;
            return registerResult;
        }
    }

}