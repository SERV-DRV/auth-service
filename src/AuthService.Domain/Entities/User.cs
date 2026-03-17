using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

public class User
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(25)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public bool Status { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual UserProfile UserProfile { get; set; } = null!; 

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual UserPasswordReset UserPasswordReset { get; set; } = null!;
    public virtual UserEmail UserEmail { get; set; } = null!;
}