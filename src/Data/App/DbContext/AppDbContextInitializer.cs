using System;
using System.Collections.Generic;
using System.Linq;
using Cayent.Core.Data.Identity.Models.Users;
using Cayent.Core.Data.Users;
using Data.App.Models.Brands;
using Data.App.Models.Customers;
using Data.App.Models.Drugs;
using Data.App.Models.Pharmacies;
using Data.App.Models.Users;
using Data.Constants;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data.App.DbContext
{
    public static class AppDbContextInitializer
    {
        static Random _rnd = new Random((int)DateTime.UtcNow.Ticks);

        public static void Initialize(IdentityWebContext identityWebContext, AppDbContext ctx, IEnumerable<ProvisionUserRole> provisionUserRoles)
        {
            if (ctx.Users.Any())
                return;

            var pharmacy = CreatePharmacy();

            CreateRoles(ctx, pharmacy);

            CopyIdentityUserToApp(identityWebContext, ctx, pharmacy);

            ctx.Add(pharmacy);


            CreateAllegre(identityWebContext, ctx);

            ctx.SaveChanges();
        }

        static Pharmacy CreatePharmacy()
        {
            var pharmacy = new Pharmacy
            {
                PharmacyId = "default",
                Name = "Default Pharmacy",
                Address = "123 Main Street",

            };

            pharmacy.Brands.Add(new Models.Brands.Brand
            {
                BrandId = "default",
                PharmacyId = pharmacy.PharmacyId,
                Name = "Default"
            });

            //pharmacy.Drugs.Add(new Models.Drugs.Drug
            //{
            //    DrugId =NewId(),
            //    Name = "Sample #1",
            //    BrandId = "default",
            //    Classification = Enums.EnumDrugClassification.OverTheCounter,
            //    IsAvailable = true,
            //    Stock=1,
            //    SafetyStock =1,
            //    ReorderLevel=1,
            //    Prices = new List<DrugPrice>
            //    {
            //        new DrugPrice
            //        {
            //            DrugId="dru"
            //        }
            //    }

            //});;

            return pharmacy;
        }

        static void CreateRoles(AppDbContext ctx, Pharmacy pharmacy)
        {
            var roles = ApplicationRoles.Items
                .Select(e => new Role
                {
                    RoleId = e.Id,
                    Name = e.Name,
                });

            ctx.AddRange(roles);
        }

        static void CopyIdentityUserToApp(IdentityWebContext identityWebContext, AppDbContext appDbContext, Pharmacy pharmacy)
        {
            var users = identityWebContext.Users.Include(e => e.UserInformation).ToList();

            var appUsers = new List<User>();

            users.ForEach(u =>
            {
                var appUser = new User
                {
                    UserId = u.Id,
                    FirstName = u.UserInformation.FirstName,
                    MiddleName = u.UserInformation.MiddleName,
                    LastName = u.UserInformation.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                };

                var userRoles = identityWebContext.UserRoles.Where(e => e.UserId == u.Id).ToList();

                appUser.UserRoles = userRoles.Select(e => new UserRole
                {
                    UserId = e.UserId,
                    RoleId = e.RoleId
                }).ToList<UserRoleBase>();

                appUsers.Add(appUser);

                // if Owner or Staff
                if (userRoles.Any(e => e.RoleId == ApplicationRoles.Administrator.Id || e.RoleId == ApplicationRoles.Staff.Id))
                {
                    var ownerOrStaff = new PharmacyStaff
                    {
                        PharmacyId = pharmacy.PharmacyId,
                        Staff = new Staff
                        {
                            StaffId = appUser.UserId
                        }
                    };

                    appDbContext.Add(ownerOrStaff);
                }

                // if rider
                if (userRoles.Any(e => e.RoleId == ApplicationRoles.Customer.Id))
                {
                    var customer = new Customer
                    {
                        CustomerId = appUser.UserId,
                    };

                    appDbContext.Add(customer);
                }
            });

            appDbContext.AddRange(appUsers);
        }

        static List<Tuple<string, string, string, string>> GetNames()
        {
            var list = new List<Tuple<string, string, string, string>>();

            //list.Add(new Tuple<string, string, string, string>("Juan", "Dela Cruz", "09191234567", "105 Paz Street, Barangay 11, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Pening", "Garcia", "09191234567", "101 Subdivision 202 Street, Barangay, Town, City, Philippines"));
            //list.Add(new Tuple<string, string, string, string>("Nadia", "Cole", "09191234567", "301 Main Street, Barangay 3, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Chino", "Pacia", "09191234567", "202 Subdivision 303 Street, Barangay, Town, City, Philippines"));
            //list.Add(new Tuple<string, string, string, string>("Vina", "Ruruth", "09191234567", "501 Main Street, Barangay 5, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Lina", "Mutac", "09191234567", "601 Main Street, Barangay 6, Balayan, Batangas City"));

            list.Add(new Tuple<string, string, string, string>("Pening", "Garcia", "09191234567", "101 Subdivision 202 Street, Barangay, Town, City, Philippines"));
            list.Add(new Tuple<string, string, string, string>("Chino", "Pacia", "09191234567", "202 Subdivision 303 Street, Barangay, Town, City, Philippines"));

            return list;
        }

        static string NewId()
        {
            return Guid.NewGuid().ToString().ToLower();
        }

        static string NewCouponCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = _rnd;
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        static void CreateAllegre(IdentityWebContext identityWebContext, AppDbContext ctx)
        {
            #region Owner

            var email = "user2@pharmaapp.com";
            var token = Guid.NewGuid().ToString();
            var admin = new IdentityWebUser
            {
                Id = "administrator2",
                UserName = email,
                NormalizedUserName = email.ToUpper(),

                Email = email,
                NormalizedEmail = email.ToUpper(),
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
                ConcurrencyStamp = token,
                UserInformation = new UserInformation
                {
                    FirstName = "Tina",
                    LastName = "Moran",
                    ConcurrencyToken = token,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };
            var adminRoles = ApplicationRoles.Items.Select(e => new IdentityUserRole<string>
            {
                UserId = admin.Id,
                RoleId = e.Id
            }).ToList();

            ctx.AddRange(admin);
            ctx.AddRange(adminRoles);

            #endregion

            var pharmacy = new Pharmacy
            {
                PharmacyId = NewId(), 
                Name = "Allegre Drugstore"
            };

            var defaultBrand = new Brand
            {
                PharmacyId = pharmacy.PharmacyId,
                BrandId = NewId(),
                Name = "Default"
            };

            var drugs = new List<Drug>()
            {
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerkid Drops",
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 160,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerkid Syrup 30ml", 
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 146,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerkid Syrup 60ml",
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 262,
                        }
                    }
                }
                ,
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerta Syrup 60ml",
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 206,
                        }
                    }
                }
            };

            pharmacy.Brands.Add(defaultBrand);
            pharmacy.Drugs = drugs;

            ctx.Add(pharmacy);
        }
    }
}
