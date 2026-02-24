using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs.Email;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;
}