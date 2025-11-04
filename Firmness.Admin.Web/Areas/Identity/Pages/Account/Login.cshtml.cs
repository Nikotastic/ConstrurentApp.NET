using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Firmness.Infrastructure.Identity; // <-- ApplicationUser

namespace Firmness.Admin.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        
        [BindProperty]
        public InputModel Input { get; set; }  = null!; 

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; }= string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = null!;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email!);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User or password is invalid.");
                return Page();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: false);
            if (signInResult.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Client"))
                {
                    // client dont have access to admin panel -> redirect to frontend
                    return LocalRedirect("~/");
                }

                await _signInManager.SignInAsync(user, Input.RememberMe);
                _logger.LogInformation("User logged in.");
                return LocalRedirect(ReturnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "User or password is invalid.");
            return Page();
        }
    }
}