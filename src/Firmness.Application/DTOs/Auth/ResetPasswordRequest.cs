namespace Firmness.Application.DTOs.Auth;

public class ResetPasswordRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
