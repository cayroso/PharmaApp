using App.CQRS;
using App.CQRS.Staffs.Common.Queries.Query;
using Data.App.DbContext;
using Data.App.Models.Pharmacies;
using Data.App.Models.Users;
using Data.Common;
using Data.Constants;
using Data.Identity.DbContext;
using Data.Identity.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize(Roles = ApplicationRoles.AdministratorRoleName)]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StaffsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public StaffsController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string c, int p, int s, string sf, int so)
        {
            var query = new SearchStaffQuery("", TenantId, UserId, PharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchStaffQuery, Paged<SearchStaffQuery.Staff>>(query);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetStaffByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetStaffByIdQuery, GetStaffByIdQuery.Staff>(query);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] SignInManager<IdentityWebUser> signInManager,
            [FromServices] UserManager<IdentityWebUser> userManager,
            [FromServices] IdentityWebContext identityWebContext,
            [FromServices] AppDbContext appDbContext,
            [FromServices] IEmailSender emailSender,
            [FromBody] AddUserInfo info)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityWebUser
                {
                    Id = GuidStr(),
                    TenantId = TenantId,
                    UserName = info.Email,
                    Email = info.Email,
                    PhoneNumber = info.PhoneNumber,
                    ConcurrencyStamp = GuidStr()
                };

                var userInfo = new UserInformation
                {
                    UserId = user.Id,
                    FirstName = info.FirstName,
                    MiddleName = info.MiddleName,
                    LastName = info.LastName,
                    ConcurrencyToken = GuidStr()
                };

                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = ApplicationRoles.Staff.Id
                };

                await identityWebContext.AddRangeAsync(userInfo);
                await identityWebContext.AddRangeAsync(userRole);

                var result = await userManager.CreateAsync(user, info.Password);


                if (result.Succeeded)
                {
                    var pharmacyStaff = new PharmacyStaff
                    {
                        PharmacyId = PharmacyId,
                        Staff = new Staff
                        {
                            User = new User
                            {
                                UserId = user.Id,
                                FirstName = info.FirstName,
                                MiddleName = info.MiddleName,
                                LastName = info.LastName,
                                PhoneNumber = info.PhoneNumber,
                                Email = info.Email,
                                UserRoles = new List<UserRole>
                                {
                                     new UserRole {
                                         UserId = userRole.UserId,
                                         RoleId = userRole.RoleId
                                     }
                                }
                            }
                        }
                    };

                    await appDbContext.AddRangeAsync(pharmacyStaff);

                    await identityWebContext.SaveChangesAsync();
                    await appDbContext.SaveChangesAsync();

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = string.Empty },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(info.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return Ok(user.Id);
                }

                var errors = string.Join(",", result.Errors.Select(e => e.Description));

                return BadRequest(errors);

            }

            return BadRequest();
        }

        [HttpPost("manage-roles")]
        public async Task<IActionResult> AddUserRole(
            [FromServices] IdentityWebContext identityWebContext,
            [FromServices] AppDbContext appDbContext,
            [FromBody] ManageUserRoleInfo info)
        {
            var appUser = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == info.UserId);

            appUser.ThrowIfNull();

            var currentUserRoles = await identityWebContext.UserRoles.Where(e => e.UserId == info.UserId).ToListAsync();

            //  check if this is the last administrator
            var staffIds = await appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == PharmacyId).Select(e => e.StaffId).ToListAsync();
            var adminCount = await appDbContext.UserRoles.Where(e => staffIds.Contains(e.UserId) && e.RoleId == ApplicationRoles.Administrator.Id).CountAsync();

            if (adminCount <= 1)
            {
                if (currentUserRoles.Any(e => e.RoleId == ApplicationRoles.Administrator.Id) && !info.RoleIds.Any(e => e == ApplicationRoles.Administrator.Id))
                {
                    return BadRequest("Removing the last administrator is not allowed.");
                }
            }

            if (currentUserRoles.Any())
            {
                identityWebContext.RemoveRange(currentUserRoles);
            }

            identityWebContext.UserRoles.AddRange(info.RoleIds.Select(e => new IdentityUserRole<string>
            {
                UserId = info.UserId,
                RoleId = e
            }));

            await identityWebContext.SaveChangesAsync();


            var currentAppUserRoles = await appDbContext.UserRoles.Where(e => e.UserId == info.UserId).ToListAsync();

            if (currentAppUserRoles.Any())
            {
                appDbContext.UserRoles.RemoveRange(currentAppUserRoles);
            }

            appDbContext.UserRoles.AddRange(info.RoleIds.Select(e => new UserRole
            {
                UserId = info.UserId,
                RoleId = e
            }));

            await appDbContext.SaveChangesAsync();

            return Ok();
        }

    }

    public class AddUserInfo
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ManageUserRoleInfo
    {
        [Required]
        [DisplayName("User")]
        public string UserId { get; set; }
        [Required]
        [DisplayName("Roles")]
        public List<string> RoleIds { get; set; }
    }

}
