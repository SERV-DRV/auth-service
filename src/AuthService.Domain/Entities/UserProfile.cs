using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Entities;

// Corrección: Se cambió "public class=UserProfile" por "public class UserProfile"
public class UserProfile
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; } // Se agregó '?' si puede ser nulo
    public string? Bio { get; set; }              // Se agregó '?' si puede ser nulo
    public DateTime DateOfBirth { get; set; }

    public User User { get; set; } = null!; // Se corrigió mayúscula en 'User' para convención C#
}