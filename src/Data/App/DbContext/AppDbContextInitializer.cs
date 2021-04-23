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
            CreateGener(identityWebContext, ctx);
            CreateZoe(identityWebContext, ctx);

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
            var pharmacyId = NewId();

            #region Owner in Identity

            var email = "allegre-user1@pharmaapp.com";
            var token = Guid.NewGuid().ToString();
            var admin = new IdentityWebUser
            {
                Id = "allegre-user1",
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
                    FirstName = "First",
                    LastName = "Last",
                    ConcurrencyToken = token,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };
            var adminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Administrator.Id
                },
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Staff.Id
                }
            };

            identityWebContext.AddRange(admin);
            identityWebContext.AddRange(adminRoles);

            #endregion

            #region Owner in App

            var appUser = new User
            {
                UserId = admin.Id,
                FirstName = admin.UserInformation.FirstName,
                MiddleName = admin.UserInformation.MiddleName,
                LastName = admin.UserInformation.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
            };

            appUser.UserRoles = adminRoles.Select(e => new UserRole
            {
                UserId = e.UserId,
                RoleId = e.RoleId
            }).ToList<UserRoleBase>();

            var ownerOrStaff = new PharmacyStaff
            {
                PharmacyId = pharmacyId,
                Staff = new Staff
                {
                    StaffId = appUser.UserId
                }
            };

            ctx.AddRange(appUser, ownerOrStaff);

            #endregion

            var pharmacy = new Pharmacy
            {
                PharmacyId = pharmacyId,
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
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
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
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
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
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 262,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerta Syrup 60ml",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 206,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Appebon with Iron Syrup 120ml",
                    Classification = Enums.EnumDrugClassification.PrescriptionOnly,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 425,
                        }
                    }
                }
            };

            pharmacy.Brands.Add(defaultBrand);
            pharmacy.Drugs = drugs;

            ctx.Add(pharmacy);
        }

        static void CreateGener(IdentityWebContext identityWebContext, AppDbContext ctx)
        {
            var pharmacyId = NewId();

            #region Owner in Identity

            var email = "gener-user1@pharmaapp.com";
            var token = Guid.NewGuid().ToString();
            var admin = new IdentityWebUser
            {
                Id = "gener-user1",
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
                    FirstName = "First",
                    LastName = "Last",
                    ConcurrencyToken = token,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };
            var adminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Administrator.Id
                },
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Staff.Id
                }
            };

            identityWebContext.AddRange(admin);
            identityWebContext.AddRange(adminRoles);

            #endregion

            #region Owner in App

            var appUser = new User
            {
                UserId = admin.Id,
                FirstName = admin.UserInformation.FirstName,
                MiddleName = admin.UserInformation.MiddleName,
                LastName = admin.UserInformation.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
            };

            appUser.UserRoles = adminRoles.Select(e => new UserRole
            {
                UserId = e.UserId,
                RoleId = e.RoleId
            }).ToList<UserRoleBase>();

            var ownerOrStaff = new PharmacyStaff
            {
                PharmacyId = pharmacyId,
                Staff = new Staff
                {
                    StaffId = appUser.UserId
                }
            };

            ctx.AddRange(appUser, ownerOrStaff);

            #endregion

            var pharmacy = new Pharmacy
            {
                PharmacyId = pharmacyId,
                Name = "Gener Drugstore"
            };

            var brandAmbica = new Brand
            {
                PharmacyId = pharmacy.PharmacyId,
                BrandId = NewId(),
                Name = "AMBICA"
            };
            var brandExcel = new Brand
            {
                PharmacyId = pharmacy.PharmacyId,
                BrandId = NewId(),
                Name = "EXCEL"
            };
            var brandAcresil = new Brand
            {
                PharmacyId = pharmacy.PharmacyId,
                BrandId = NewId(),
                Name = "ACRESIL"
            };

            var drugs = new List<Drug>()
            {
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandAmbica.BrandId,
                    DrugId = NewId(),
                    Name = "ACETYLCISTEIN 200MG",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 10, SafetyStock = 10, ReorderLevel = 10,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 12,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandAmbica.BrandId,
                    DrugId = NewId(),
                    Name = "ACETYLCISTEIN 600MG",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 10, SafetyStock = 10, ReorderLevel = 10,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 24,
                        }
                    }
                },

                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandExcel.BrandId,
                    DrugId = NewId(),
                    Name = "CEFALEXIN 125MG SUSP",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 3, SafetyStock = 3, ReorderLevel = 3,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 30,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandExcel.BrandId,
                    DrugId = NewId(),
                    Name = "CEFALEXIN 500MG SUSP",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 3, SafetyStock = 3, ReorderLevel = 3,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 45,
                        }
                    }
                },

                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandAcresil.BrandId,
                    DrugId = NewId(),
                    Name = "CLINDAMYCIN 150MG CAP",
                    Classification = Enums.EnumDrugClassification.PrescriptionOnly,
                    IsAvailable = true,
                    Stock = 100, SafetyStock = 100, ReorderLevel = 100,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 5,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = brandAcresil.BrandId,
                    DrugId = NewId(),
                    Name = "CLINDAMYCIN 300MG CAP",
                    Classification = Enums.EnumDrugClassification.PrescriptionOnly,
                    IsAvailable = true,
                    Stock = 100, SafetyStock = 100, ReorderLevel = 100,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 8,
                        }
                    }
                }
            };

            pharmacy.Brands.Add(brandExcel);
            pharmacy.Brands.Add(brandAmbica);
            pharmacy.Brands.Add(brandAcresil);

            pharmacy.Drugs = drugs;

            ctx.Add(pharmacy);
        }

        static void CreateZoe(IdentityWebContext identityWebContext, AppDbContext ctx)
        {
            var pharmacyId = NewId();

            #region Owner in Identity

            var email = "zoe-user1@pharmaapp.com";
            var token = Guid.NewGuid().ToString();
            var admin = new IdentityWebUser
            {
                Id = "zoe-user1",
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
                    FirstName = "First",
                    LastName = "Last",
                    ConcurrencyToken = token,
                    Theme = "https://bootswatch.com/4/journal/bootstrap.min.css"
                }
            };
            var adminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Administrator.Id
                },
                new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = ApplicationRoles.Staff.Id
                }
            };

            identityWebContext.AddRange(admin);
            identityWebContext.AddRange(adminRoles);

            #endregion

            #region Owner in App

            var appUser = new User
            {
                UserId = admin.Id,
                FirstName = admin.UserInformation.FirstName,
                MiddleName = admin.UserInformation.MiddleName,
                LastName = admin.UserInformation.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
            };

            appUser.UserRoles = adminRoles.Select(e => new UserRole
            {
                UserId = e.UserId,
                RoleId = e.RoleId
            }).ToList<UserRoleBase>();

            var ownerOrStaff = new PharmacyStaff
            {
                PharmacyId = pharmacyId,
                Staff = new Staff
                {
                    StaffId = appUser.UserId
                }
            };

            ctx.AddRange(appUser, ownerOrStaff);

            #endregion

            var pharmacy = new Pharmacy
            {
                PharmacyId = pharmacyId,
                Name = "Zoe Drugstore"
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
                    Name = "ALAXAN FR CAP 10 SS PH",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 75.31,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "ALAXAN TAB 10 SS PH C",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 76.19,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "ALLERKID 5 MG SYR 60ML SS PH",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 245.16,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "Allerta Syrup 60ml",
                    Classification = Enums.EnumDrugClassification.OverTheCounter,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 206,
                        }
                    }
                },
                new Drug
                {
                    PharmacyId = pharmacy.PharmacyId,
                    BrandId = defaultBrand.BrandId,
                    DrugId = NewId(),
                    Name = "ALLERZET 5MG TAB 100BOX SS PH",
                    Classification = Enums.EnumDrugClassification.PrescriptionOnly,
                    IsAvailable = true,
                    Stock = 1, SafetyStock = 1, ReorderLevel = 1,
                    Prices = new List<DrugPrice>
                    {
                        new DrugPrice
                        {
                            DrugPriceId = NewId(),
                            Price= 2078.74,
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
