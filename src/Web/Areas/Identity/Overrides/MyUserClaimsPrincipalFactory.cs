﻿using Cayent.Core.Common.Extensions;
using Cayent.Core.Data.Identity.Models;
using Cayent.Core.Data.Identity.Models.Users;
using Data.App.DbContext;
using Data.Identity.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Areas.Identity.Overrides
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityWebUser, IdentityRole>
    {
        readonly AppDbContext _appDbContext;
        readonly IdentityWebContext _identityWebContext;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly IHttpContextAccessor _accessor;
        public MyUserClaimsPrincipalFactory(
            UserManager<IdentityWebUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            AppDbContext appDbContext,
            IdentityWebContext identityWebContext,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor accessor
            )
            : base(userManager, roleManager, optionsAccessor)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _identityWebContext = identityWebContext ?? throw new ArgumentNullException(nameof(identityWebContext));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityWebUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var remoteIpAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            var loginAudit = new LoginAudit
            {
                LoginDate = DateTime.UtcNow.Truncate().AsUtc(),
                UserId = user.Id,
                RemoteIpAddress = remoteIpAddress,
            };

            await _identityWebContext.AddAsync(loginAudit);

            await _identityWebContext.SaveChangesAsync();

            //var appDbUser = await _appDbContext.Accounts.FirstOrDefaultAsync(p => p.AccountId == user.Id);	

            //if (appDbUser != null)	
            //{	
            //    identity.AddClaim(new Claim("FullName", user.FirstLastName));	

            //    if (!string.IsNullOrWhiteSpace(appDbUser.ProfilePicture32))	
            //        identity.AddClaim(new Claim("ProfilePicture32", appDbUser.ProfilePicture32));	
            //    else	
            //        identity.AddClaim(new Claim("ProfilePicture32", "no picture"));	

            //    identity.AddClaim(new Claim("Initials", user.Initials));	
            //}	

            var tenant = await _identityWebContext
                .Tenants
                //.Include(e => e.Users)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TenantId == user.TenantId);

            if (!string.IsNullOrWhiteSpace(user.TenantId))
            {
                identity.AddClaim(new Claim("TenantId", user.TenantId));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user..FirstLastName));
                identity.AddClaim(new Claim("TenantName", tenant.Name));
            }
            var userInfo = await _identityWebContext.UserInformations.FirstOrDefaultAsync(e => e.UserId == user.Id);
            var theme = userInfo.Theme;
            if (theme == null)
                theme = string.Empty;
            identity.AddClaim(new Claim("Profile:Theme", theme));

            var urlProfilePicture = "";

            if (!string.IsNullOrWhiteSpace(userInfo.ImageId))
            {
                urlProfilePicture = $"/api/files/{userInfo.ImageId}";
            }

            identity.AddClaim(new Claim("Profile:UrlProfilePicture", urlProfilePicture));

            var pharmacyStaff = await _appDbContext.PharmacyStaffs.FirstOrDefaultAsync(e => e.StaffId == user.Id);

            if (pharmacyStaff != null)
            {
                identity.AddClaim(new Claim("PharmacyId", pharmacyStaff.PharmacyId));
            }

            //_appDbContextFactory.Provision(tenant, _webHostEnvironment.IsDevelopment());

            return identity;
        }
    }
}
