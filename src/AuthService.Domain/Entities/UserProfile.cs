using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // This namespace is required for the [ForeignKey] attribute used in the UserProfile class.

namespace AuthService.Domain.Entities;

public class UserProfile
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserId is required.")]
    [MaxLength(16)]
    [ForeignKey(nameof(User))] // This property is a foreign key that references the User entity. It establishes a relationship between the UserProfile and User entities, indicating that each UserProfile is associated with a specific User.
    public string UserId { get; set; } = string.Empty;

    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public User User { get; set; } = null!;
}