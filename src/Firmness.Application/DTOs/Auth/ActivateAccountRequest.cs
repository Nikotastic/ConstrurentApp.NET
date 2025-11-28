namespace Firmness.Application.DTOs.Auth;

public class ActivateAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
