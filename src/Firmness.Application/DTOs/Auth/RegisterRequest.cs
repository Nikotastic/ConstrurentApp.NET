namespace Firmness.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // Customer fields
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Document { get; set; }
    public string Phone { get; set; }
    public string? Address { get; set; }
}
