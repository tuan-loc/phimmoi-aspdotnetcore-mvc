using PhimMoi.Application.Models;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using System.Security.Claims;

namespace PhimMoi.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindByIdAsync(string id);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> GetByClaims(ClaimsPrincipal claims);
        Task<User?> GetUserWithLikedMovies(string id);
        bool IsSignIn(ClaimsPrincipal claims);
        Task<bool> IsInRoleAsync(User? user, string role);
        Task<bool> IsLockedOutAsync(User user);
        Task<PagedList<User>> SearchAsync(PagingParameter pagingParameter, string? value = null, string? role = null);
        Task<IEnumerable<string>> GetRolesAsync(User user);
        Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<Result> ChangeEmailAsync(string userId, string email);
        Task<Result> ChangeUserRoleAsync(string userId, string? role);
        Task<Result> ToggleLockUserAsync(string userId);
        Task<Result> UpdateAsync(string userId, User user);
        Task<Result> DeleteAsync(string userId);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<Result> ResetPasswordAsync(string userEmail, string code, string newPassword);
        Task<Result> ConfirmEmailAsync(string userId, string token);
    }
}