using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

//? Por que IUserRepository da error?
// El error se debe a que IUserRepository es una interfaz y no se puede instanciar directamente. Para solucionar este error, debes asegurarte de que UserRepository implemente la interfaz IUserRepository correctamente. Además, debes registrar UserRepository como una implementación de IUserRepository en el contenedor de dependencias de tu aplicación para que pueda ser inyectada donde sea necesario.
public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await context.Users
        .Include(u => u.UserProfile)
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        // Forma en la que user buscara el rol y luego el rol buscara el usuario
        .FirstOrDefaultAsync(u => u.Id == id);
        return user ?? throw new InvalidOperationException($"User with id {id} not found.");
    }

    // 2. Busca un usuario por su Email (sin importar mayúsculas o minúsculas)
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
    }
    // 3. Busca un usuario por su Username (sin importar mayúsculas o minúsculas)
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
        .Include(u => u.UserProfile)
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Username, username));
    }

    public async Task<bool> ExistByUsernameAsync(string username)
    {
        // Verifica si existe al menos uno que coincida
        return await context.Users
            .AnyAsync(u => EF.Functions.ILike(u.Username, username));
    }

    // 4. Busca un usuario mediante su token de verificacion de correo
    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserEmail != null &&
                u.UserEmail.EmailVerificationToken == token);
    }

    // 5. Busca un usuario mediante su token de reseteo de contraseña
    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserPasswordReset != null &&
                u.UserPasswordReset.PasswordResetToken == token);
    }

    // 6. Crea un nuevo registro de usuario en la DB y lo retorna con sus relaciones
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var createdUser = await GetByIdAsync(user.Id);
        if (createdUser == null)
        {
            throw new InvalidOperationException("No se pudo encontrar el usuario recién creado.");
        }
        return createdUser;
    }

    // 7. Actualiza la información de un usario existente
    public async Task<User> UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
        var updatedUser = await GetByIdAsync(user.Id);
        if (updatedUser == null)
        {
            throw new InvalidOperationException("No se pudo encontrar el usuario actualizado.");
        }
        return updatedUser;
    }

    // 8. Elimina un usario de la DB por su Id
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {   
            throw new InvalidOperationException($"User with id {id} not found.");            
        }
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    // 9. Verifica si un email ya está registrado
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Email, email));
    }

    // 10. Verifica si un username ya está registrado
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Username, username));
    }

    // 11. Cambia el rol de un usuario: elimina roles previos y asigna uno nuevo
    public async Task UpdateUserRoleAsync(string userId, string roleId)
    {
        var existingRoles = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        context.UserRoles.RemoveRange(existingRoles);

        var newUserRole = new UserRole
        {
            Id = UuidGenerator.GenerateUserId(),
            UserId = userId,
            RoleId = roleId
        };
        context.UserRoles.Add(newUserRole);
        await context.SaveChangesAsync();
    }
}