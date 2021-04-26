using Cayent.Core.Data.Identity.Models.Users;
using Data.Constants;
using Data.Identity.Models.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Identity.DbContext
{
    public static class IdentityWebContextInitializer
    {
        public static void Initialize(IdentityWebContext ctx)
        {
            if (ctx.Users.Any())
                return;

            CreateRoles(ctx);

            CreateAdministrator(ctx);

            CreateCustomer(ctx);

            ctx.SaveChanges();
        }

        static void CreateRoles(IdentityWebContext ctx)
        {
            var roles = ApplicationRoles.Items.Select(e => new IdentityRole
            {
                Id = e.Id,
                Name = e.Name,
                NormalizedName = e.Name.ToUpper()
            });

            ctx.AddRange(roles);
        }

        static void CreateAdministrator(IdentityWebContext ctx)
        {
            //var tenant = new Tenant
            //{
            //    TenantId = "administrator",
            //    Name = "Administrator",
            //    Host = "Administrators Host",
            //    DatabaseConnectionString = @"Data Source=App_Data\TenantDB-DV.db;",
            //    PhoneNumber = "+639198262335",
            //    Email = "caydev2010@gmail.com",
            //    Address = "",

            //};

            var email1 = "user1@pharmaapp.com";
            var token1 = Guid.NewGuid().ToString();
            var admin1 = new IdentityWebUser
            {
                Id = "administrator1",
                UserName = email1,
                NormalizedUserName = email1.ToUpper(),

                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+639198262335",
                PhoneNumberConfirmed = true,

                LockoutEnabled = false,
                LockoutEnd = null,
                PasswordHash = "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg==",
                SecurityStamp = "6SADCY3NMMLOHA2S26ZJCEWGHWSQUYRM",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                //TenantId = tenant.TenantId,
                ConcurrencyStamp = token1,
                UserInformation = new UserInformation
                {
                    FirstName = "Admin1",
                    LastName = "Admin1",
                    ConcurrencyToken = token1,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };

            //var admin1Roles = ApplicationRoles.Items.Select(e => new IdentityUserRole<string>
            //{
            //    UserId = admin1.Id,
            //    RoleId = e.Id
            //}).ToList();

            var admin1Roles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    UserId = admin1.Id,
                    RoleId = ApplicationRoles.Systems.Id
                }
            };

            ctx.AddRange(admin1);
            ctx.AddRange(admin1Roles);

            var email2 = "user2@pharmaapp.com";
            var admin2 = new IdentityWebUser
            {
                Id = "administrator2",
                UserName = email2,
                NormalizedUserName = email2.ToUpper(),

                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+639198262335",
                PhoneNumberConfirmed = true,

                LockoutEnabled = false,
                LockoutEnd = null,
                PasswordHash = "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg==",
                SecurityStamp = "6SADCY3NMMLOHA2S26ZJCEWGHWSQUYRM",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                //TenantId = tenant.TenantId,
                ConcurrencyStamp = token1,
                UserInformation = new UserInformation
                {
                    FirstName = "Admin2",
                    LastName = "Admin2",
                    ConcurrencyToken = token1,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };
            //var admin2Roles = ApplicationRoles.Items.Select(e => new IdentityUserRole<string>
            //{
            //    UserId = admin2.Id,
            //    RoleId = e.Id
            //}).ToList();

            var admin2Roles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    UserId = admin2.Id,
                    RoleId = ApplicationRoles.Systems.Id
                }
            };

            ctx.AddRange(admin2);
            ctx.AddRange(admin2Roles);
        }

        static void CreateCustomer(IdentityWebContext ctx)
        {
            var emailPasswords = new List<Tuple<string, string, string, string>>
            {
                new Tuple<string, string, string, string>("customer1@pharmmaapp.com","Kerina","Talandipa", "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg=="),
                new Tuple<string, string, string, string>("customer2@pharmmaapp.com","Tina","Moran", "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg=="),
                new Tuple<string, string, string, string>("customer3@pharmmaapp.com","Pening","Garcia", "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg=="),
                new Tuple<string, string, string, string>("customer4@pharmmaapp.com","Chino","Pacia", "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg=="),
            };

            foreach (var ep in emailPasswords)
            {
                var userId = Guid.NewGuid().ToString();
                var email = ep.Item1;
                var fname = ep.Item2;
                var lname = ep.Item3;
                var pwd = ep.Item4;
                var token = Guid.NewGuid().ToString();

                var customer = new IdentityWebUser
                {
                    Id = userId,
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),

                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumber = "+639198262335",
                    PhoneNumberConfirmed = true,

                    LockoutEnabled = false,
                    LockoutEnd = null,
                    PasswordHash = pwd,
                    SecurityStamp = "6SADCY3NMMLOHA2S26ZJCEWGHWSQUYRM",
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0,
                    //TenantId = tenant.TenantId,
                    ConcurrencyStamp = token,
                    UserInformation = new UserInformation
                    {
                        FirstName = fname,
                        LastName = lname,
                        ConcurrencyToken = token,
                        Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                    }
                };
                var customerRole = new IdentityUserRole<string>
                {
                    UserId = userId,
                    RoleId = ApplicationRoles.Customer.Id
                };


                ctx.AddRange(customer, customerRole);
            }
        }
    }
}
