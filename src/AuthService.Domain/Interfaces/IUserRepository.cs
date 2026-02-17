using AuthService.Domain.Entities;
namespace AuthService.Domain.Interfaces;

public interface IUserRepository{
    //MÉTODOS DE CONSULTA
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByPasswordResteTokenAsync(string token);
    Task<User?> CreateAsync(User users);

    Task<bool> ExistByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task UpdateUserRoleAsync(string userId, string roleId);
}