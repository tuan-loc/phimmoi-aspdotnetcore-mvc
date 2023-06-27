namespace PhimMoi.Application.Interfaces
{
    public interface IRoleService
    {
        public Task<IEnumerable<string>> GetRolesAsync();
    }
}