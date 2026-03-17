using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Entities;

public class UserProfile
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    [ForeignKey(nameof(User))]
    public String UserId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public User Users { get; set; } = null!;
}