using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Interfaces;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailTestController : ControllerBase // <--- Asegúrate que diga public
{
    private readonly IEmailService _emailService;

    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-welcome")]
    public async Task<IActionResult> TestWelcome(string email, string name)
    {
        await _emailService.SendWelcomeEmailAsync(email, name);
        return Ok(new { message = $"¡Éxito! Correo enviado a {email}" });
    }
}