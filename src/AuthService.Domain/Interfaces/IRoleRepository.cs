using AuthService.Domain.Entities;
namespace AuthService.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<int> CountUsersInRoleAsync(string roleId);
    Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string userId);
}
    