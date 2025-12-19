using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Firmness.Infrastructure.Identity;

public class UserAccountService : IUserAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;

    // Constructor
    public UserAccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Create user and generate activation token
    public async Task<(string UserId, string Token)> CreateUserAndGenerateActivationTokenAsync(string email, string firstName, string lastName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // Imported by admin, assumed verified or will be verified by receiving the email
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Add to Client role
            await _userManager.AddToRoleAsync(user, "Client");
        }

        // Generate token for setting password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        return (user.Id, token);
    }
}
