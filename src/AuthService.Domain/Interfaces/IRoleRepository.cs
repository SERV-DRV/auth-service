using AuthService.Domain.Entities;
namespace AuthService.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<int> CountUsersInRoleAsync(string role);
    Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid userId);
}