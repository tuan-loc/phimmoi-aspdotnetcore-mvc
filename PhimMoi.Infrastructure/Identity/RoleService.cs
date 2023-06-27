using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhimMoi.Application.Interfaces;

namespace PhimMoi.Infrastructure.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            return (await _roleManager.Roles.ToListAsync()).Select(r => r.Name);
        }
    }
}