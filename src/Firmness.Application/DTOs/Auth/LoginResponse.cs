namespace Firmness.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; }
}
