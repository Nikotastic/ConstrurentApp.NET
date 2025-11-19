using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación JWT
public class ProfileController : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Policy = "RequireClientRole")] // Solo clientes autenticados
    public IActionResult GetMyProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            userId,
            email,
            username,
            roles,
            message = "¡Bienvenido! Tu token JWT funciona correctamente."
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Policy = "RequireAdminRole")] // Solo administradores
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "Este endpoint solo es accesible para administradores.",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("client-only")]
    [Authorize(Policy = "RequireClientRole")] // Solo clientes
    public IActionResult ClientOnly()
    {
        return Ok(new
        {
            message = "Este endpoint solo es accesible para clientes.",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("authenticated")]
    public IActionResult Authenticated()
    {
        // Cualquier usuario autenticado (con token válido)
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return Ok(new
        {
            message = "Estás autenticado. Este endpoint acepta cualquier usuario con token válido.",
            yourRoles = roles
        });
    }

    [HttpGet("public")]
    [AllowAnonymous] // Sin autenticación requerida
    public IActionResult Public()
    {
        return Ok(new
        {
            message = "Este endpoint es público, no requiere token."
        });
    }
}

