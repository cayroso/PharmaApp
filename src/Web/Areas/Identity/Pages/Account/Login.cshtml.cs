using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Cayent.Core.Data.Identity.Models.Users;
using Data.Identity.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityWebUser> _userManager;
        private readonly SignInManager<IdentityWebUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityWebUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityWebUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<EmailRoles> EmailRoles { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync([FromServices] IdentityWebContext webContext, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            //  get all existing user in the system
            await GetEmailRoles(webContext);
        }

        public async Task<IActionResult> OnPostAsync([FromServices] IdentityWebContext webContext, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    await GetEmailRoles(webContext);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        async Task GetEmailRoles(IdentityWebContext webContext)
        {
            var sql = from u in webContext.Users
                      join ur in webContext.UserRoles on u.Id equals ur.UserId
                      join r in webContext.Roles on ur.RoleId equals r.Id
                      select new
                      {
                          u.Email,
                          r.Name
                      };

            var dto = await sql.ToListAsync();

            EmailRoles = dto.GroupBy(e => e.Email).Select(e =>
                new EmailRoles
                {
                    Email = e.Key,
                    Roles = e.Select(p => p.Name).OrderBy(e => e).ToList()
                })
                .OrderByDescending(e => e.Roles.Count).ThenBy(e => e.Email)
                .ToList();
        }
    }

    public class EmailRoles
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
