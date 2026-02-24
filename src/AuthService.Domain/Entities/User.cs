using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

public class User
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required.")]
    [MaxLength(16)]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")] // The value of this property must have an email format.
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [MaxLength(25)]
    public bool Status { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    // Relationships
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public UserEmail UserEmail { get; set; } = null!;
    public UserPasswordReset UserPasswordReset { get; set; } = null!;
}