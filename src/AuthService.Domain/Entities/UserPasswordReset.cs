using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;
public class UserPasswordReset
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;
 
    [Required]
    [MaxLength(16)]
    [ForeignKey(nameof(User))] // Clave foránea que referencia a la entidad User, indicando que cada restablecimiento de contraseña está asociado a un usuario específico.
    public string UserId { get; set; } = string.Empty;
 
    [MaxLength(256)]
    public string? PasswordResetToken { get; set; }
 
    public DateTime? PasswordResetTokenExpiry { get; set; }
 
    [Required]
    public User User { get; set; } = null!;
}