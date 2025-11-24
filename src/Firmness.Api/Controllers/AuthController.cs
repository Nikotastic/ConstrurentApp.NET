using Firmness.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly ICustomerService _customerService;
    private readonly INotificationService _notificationService;

    public AuthController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration, 
        ILogger<AuthController> logger,
        ICustomerService customerService,
        INotificationService notificationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _customerService = customerService;
        _notificationService = notificationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        try 
        {
            _logger.LogInformation("Login attempt for {Email}", model.Email);

            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Invalid model state for {Email}", model.Email);
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", model.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User found, checking password for {Email}", model.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Invalid password for {Email}", model.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check if customer is active
            var customer = await _customerService.GetByIdentityUserIdAsync(user.Id);
            if (customer != null && !customer.IsActive)
            {
                _logger.LogWarning("Login blocked: Customer account is inactive for {Email}", model.Email);
                return Unauthorized(new { message = "Your account is inactive. Please contact support." });
            }

            _logger.LogInformation("Password correct, getting roles for {Email}", model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            
            _logger.LogInformation("Generating token for {Email}", model.Email);
            var token = GenerateJwtToken(user, roles);

            _logger.LogInformation("Login successful for {Email}", model.Email);
            return Ok(new LoginResponse { Token = token, ExpiresInSeconds = 3600 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}. StackTrace: {StackTrace}", model.Email, ex.StackTrace);
            return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}", details = ex.ToString() });
        }
    }

    [HttpPost("register-client")]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null) return Conflict(new { message = "Email already in use" });

        var existingUsername = await _userManager.FindByNameAsync(model.Username);
        if (existingUsername != null) return Conflict(new { message = "Username already in use" });

        // 1. Create AuthUser for authentication
        var user = new ApplicationUser { UserName = model.Username, Email = model.Email, EmailConfirmed = true };
        var create = await _userManager.CreateAsync(user, model.Password);
        if (!create.Succeeded)
        {
            return BadRequest(new { errors = create.Errors.Select(e => e.Description) });
        }

        // 2. Add Client role
        var roleResult = await _userManager.AddToRoleAsync(user, "Client");
        if (!roleResult.Succeeded)
        {
            // Rollback: delete the created user
            await _userManager.DeleteAsync(user);
            return BadRequest(new { errors = roleResult.Errors.Select(e => e.Description) });
        }

        // 3. Create Customer entity (visible in dashboard)
        try
        {
            var customer = new Customer(
                firstName: model.FirstName ?? model.Username,
                lastName: model.LastName ?? "",
                email: model.Email
            )
            {
                IdentityUserId = user.Id,
                Document = model.Document ?? "",
                Phone = model.Phone ?? "",
                Address = model.Address ?? "",
                IsActive = true
            };

            await _customerService.AddAsync(customer);
            
            _logger.LogInformation(
                "User successfully registered: {Username} ({Email}) - Customer ID: {CustomerId}", 
                user.UserName, 
                user.Email, 
                customer.Id);

            // Send welcome email
            try
            {
                await _notificationService.SendWelcomeEmailAsync(customer);
                _logger.LogInformation("Welcome email sent to {Email}", customer.Email);
            }
            catch (Exception emailEx)
            {
                // Don't fail registration if email fails, just log it
                _logger.LogError(emailEx, "Failed to send welcome email to {Email}", customer.Email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Customer for user {UserId}", user.Id);
            // Rollback: delete the created user
            await _userManager.DeleteAsync(user);
            return StatusCode(500, new { message = "Error creating customer profile. Registration cancelled." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new LoginResponse { Token = token, ExpiresInSeconds = 3600 });
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        // Use the same long default as in Program.cs to avoid weak-key exceptions.
        var jwtKey = Environment.GetEnvironmentVariable("JWT__Key") ?? _configuration["Jwt:Key"] ?? "dev_default_change_me_to_a_strong_secret_please_!2025";
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? _configuration["Jwt:Issuer"] ?? "Firmness.Api";
        var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? _configuration["Jwt:Audience"] ?? "FirmnessClients";

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException($"JWT key is too short ({keyBytes.Length * 8} bits). The key must be at least 256 bits (32 bytes). Set environment variable JWT__Key or configuration 'Jwt:Key' with a sufficiently long secret.");
        }

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // DTOs
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

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

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
    }
    
}