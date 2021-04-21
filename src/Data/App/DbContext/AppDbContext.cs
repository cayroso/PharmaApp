
using Cayent.Core.CQRS.Services;
using Cayent.Core.Data.Chats;
using Cayent.Core.Data.Fileuploads;
using Cayent.Core.Data.Identity.Models;
using Cayent.Core.Data.Notifications;
using Data.App.Models.Brands;
using Data.App.Models.Calendars;

using Data.App.Models.Customers;
using Data.App.Models.Drugs;

using Data.App.Models.Orders;
using Data.App.Models.Orders.OrderLineItems;
using Data.App.Models.Pharmacies;
using Data.App.Models.Users;
using Data.Identity.Models;
using Data.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Data.App.DbContext
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Get environment	
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory());
            Console.WriteLine($"environment: {environment}");
            Console.WriteLine($"appSettingsPath: {appSettingsPath}");

            // Build config	
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            return new AppDbContext(optionsBuilder.Options, config, null);
        }
    }

    public class AppDbContext : AppBaseDbContext//Microsoft.EntityFrameworkCore.DbContext
    {
        #region configuration

        readonly bool _useSQLite;
        readonly string _connString;

        #endregion


        const int KeyMaxLength = 36;
        const int NameMaxLength = 256;
        const int DescMaxLength = 2048;
        const int NoteMaxLength = 4096;

        private Tenant _tenant;

        private readonly IConfiguration _configuration;


        public DbSet<Brand> Brands { get; set; }

        public DbSet<Calendar> Calendars { get; set; }

        //public new DbSet<Chat> Chats { get; set; }
        //public new DbSet<ChatMessage> ChatMessages { get; set; }
        //public new DbSet<ChatReceiver> ChatReceivers { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Drug> Drugs { get; set; }

        //public DbSet<FileUpload> FileUploads { get; set; }

        //public DbSet<Notification> Notifications { get; set; }
        //public DbSet<NotificationReceiver> NotificationReceivers { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderTimeline> OrderTimelines { get; set; }

        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<PharmacyStaff> PharmacyStaffs { get; set; }

        public new DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<UserTaskItem> UserTaskItems { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration, ITenantProvider tenantProvider)
            : base(options)
        {
            _configuration = configuration;

            if (tenantProvider != null)
                _tenant = tenantProvider.GetTenant();


            _useSQLite = _configuration.GetValue<bool>("AppSettings:UseSQLite");

            _connString = _useSQLite ? _configuration.GetConnectionString("AppDbContextConnectionSQLite") : _configuration.GetConnectionString("AppDbContextConnectionSQLServer");

            if (_tenant != null)
            {
                _connString = _tenant.DatabaseConnectionString;
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_useSQLite)
            {
                optionsBuilder.UseSqlite(_connString);
            }
            else
            {
                optionsBuilder.UseSqlServer(_connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            CreateBrands(builder);

            CreateCalendar(builder);

            CreateChats(builder);

            CreateCustomers(builder);

            CreateDrugs(builder);

            CreateFileUploads(builder);

            CreateNotifications(builder);

            CreateOrders(builder);

            CreatePharmacies(builder);

            CreateUser(builder);
        }

        void CreateBrands(ModelBuilder builder)
        {
            builder.Entity<Brand>(b =>
            {
                b.ToTable("Brand");
                b.HasKey(e => e.BrandId);

                b.Property(e => e.BrandId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Name).HasMaxLength(NameMaxLength).IsRequired();

                b.HasMany(e => e.Drugs)
                    .WithOne(d => d.Brand)
                    .HasForeignKey(fk => fk.BrandId);
            });

        }

        void CreateCalendar(ModelBuilder builder)
        {
            builder.Entity<Calendar>(b =>
            {
                b.ToTable("Calendar");
                b.HasKey(e => e.Date);

                b.Property(e => e.MonthName).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DayName).HasMaxLength(KeyMaxLength).IsRequired();
            });

        }

        void CreateChats(ModelBuilder builder)
        {
            builder.Entity<Chat>(b =>
            {
                b.ToTable("Chat");
                b.HasKey(p => p.ChatId);

                b.Property(p => p.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(p => p.LastChatMessageId).HasMaxLength(KeyMaxLength);
                b.Property(p => p.Title).HasMaxLength(NameMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Receivers)
                    .WithOne(d => d.Chat)
                    .HasForeignKey(f => f.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(e => e.Messages)
                    .WithOne(d => d.Chat)
                    .HasForeignKey(f => f.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ChatMessage>(b =>
            {
                b.ToTable("ChatMessage");
                b.HasKey(p => p.ChatMessageId);

                b.Property(e => e.ChatMessageId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.SenderId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.Content).HasMaxLength(DescMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<ChatReceiver>(b =>
            {
                b.ToTable("ChatReceiver");
                b.HasKey(p => p.ChatReceiverId);

                b.Property(e => e.ChatReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.LastChatMessageId).HasMaxLength(KeyMaxLength);

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();
            });


        }

        void CreateCustomers(ModelBuilder builder)
        {
            builder.Entity<Customer>(b =>
            {
                b.ToTable("Customer");
                b.HasKey(e => e.CustomerId);

                b.Property(e => e.CustomerId).HasMaxLength(KeyMaxLength).IsRequired();
                //b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasOne(e => e.User).WithOne().HasForeignKey<Customer>(e => e.CustomerId);

                b.HasMany(e => e.Orders)
                    .WithOne(d => d.Customer)
                    .HasForeignKey(f => f.CustomerId);
            });
        }

        void CreateDrugs(ModelBuilder builder)
        {
            builder.Entity<Drug>(b =>
            {
                b.ToTable("Drug");
                b.HasKey(e => e.DrugId);

                b.Property(e => e.DrugId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.BrandId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Prices)
                    .WithOne(d => d.Drug)
                    .HasForeignKey(f => f.DrugId);

                b.HasMany(e => e.CustomerSubscriptions)
                    .WithOne(d => d.Drug)
                    .HasForeignKey(f => f.DrugId);

                b.HasMany(e => e.OrderLineItems)
                    .WithOne(d => d.Drug)
                    .HasForeignKey(f => f.DrugId);

                b.HasQueryFilter(e => e.Active);
            });

            builder.Entity<DrugPrice>(b =>
            {
                b.ToTable("DrugPrice");
                b.HasKey(e => e.DrugPriceId);

                b.Property(e => e.DrugPriceId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DrugId).HasMaxLength(KeyMaxLength).IsRequired();
                
                b.HasQueryFilter(e => e.Active);
            });
        }

        void CreateFileUploads(ModelBuilder builder)
        {
            builder.Entity<FileUpload>(b =>
            {
                b.ToTable("FileUpload");
                b.HasKey(e => e.FileUploadId);

                b.Property(e => e.FileUploadId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Url).HasMaxLength(DescMaxLength);
                b.Property(e => e.FileName).HasMaxLength(DescMaxLength);
                b.Property(e => e.ContentDisposition).HasMaxLength(DescMaxLength);
                b.Property(e => e.ContentType).HasMaxLength(DescMaxLength);


            });
        }

        void CreateNotifications(ModelBuilder builder)
        {
            builder.Entity<Notification>(b =>
            {
                b.ToTable("Notification");
                b.HasKey(e => e.NotificationId);

                b.Property(e => e.NotificationId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Receivers)
                    .WithOne(d => d.Notification)
                    .HasForeignKey(f => f.NotificationId);
            });

            builder.Entity<NotificationReceiver>(b =>
            {
                b.ToTable("NotificationReceiver");
                b.HasKey(e => e.NotificationReceiverId);

                b.Property(e => e.NotificationReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.NotificationId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
            });

        }


        void CreateOrders(ModelBuilder builder)
        {
            builder.Entity<Order>(b =>
            {
                b.ToTable("Order");
                b.HasKey(e => e.OrderId);

                b.Property(e => e.OrderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.CustomerId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.LineItems)
                    .WithOne(d => d.Order)
                    .HasForeignKey(f => f.OrderId);

                b.HasMany(e => e.FileUploads)
                    .WithOne(d => d.Order)
                    .HasForeignKey(f => f.OrderId);
            });

            builder.Entity<OrderLineItem>(b =>
            {
                b.ToTable("OrderLineItem");
                b.HasKey(e => e.OrderLineItemId);

                b.Property(e => e.OrderLineItemId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.OrderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DrugId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DrugPriceId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<OrderFileUpload>(b =>
            {
                b.ToTable("OrderFileUpload");
                b.HasKey(e => e.OrderFileUploadId);

                b.Property(e => e.OrderFileUploadId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.OrderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.FileUploadId).HasMaxLength(KeyMaxLength).IsRequired();                
            });

            builder.Entity<OrderTimeline>(b =>
            {
                b.ToTable("OrderTimeline");
                b.HasKey(e => e.OrderTimelineId);

                b.Property(e => e.OrderTimelineId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.OrderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
            });
        }

        void CreatePharmacies(ModelBuilder builder)
        {
            builder.Entity<Pharmacy>(b =>
            {
                b.ToTable("Pharmacy");
                b.HasKey(e => e.PharmacyId);

                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Brands)
                    .WithOne(d => d.Pharmacy)
                    .HasForeignKey(f => f.PharmacyId);

                b.HasMany(e => e.Drugs)
                    .WithOne(d => d.Pharmacy)
                    .HasForeignKey(f => f.PharmacyId);

                b.HasMany(e => e.Orders)
                    .WithOne(d => d.Pharmacy)
                    .HasForeignKey(f => f.PharmacyId);

                b.HasMany(e => e.Reviews)
                    .WithOne(d => d.Pharmacy)
                    .HasForeignKey(f => f.PharmacyId);
            });

            builder.Entity<PharmacyReview>(b =>
            {
                b.ToTable("PharmacyReview");
                b.HasKey(e => e.PharmacyReviewId);

                b.Property(e => e.PharmacyReviewId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.CustomerId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<PharmacyStaff>(b =>
            {
                b.ToTable("PharmacyStaff");
                b.HasKey(e => e.PharmacyStaffId);

                b.Property(e => e.PharmacyStaffId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PharmacyId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.StaffId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<Staff>(b =>
            {
                b.ToTable("Staff");
                b.HasKey(e => e.StaffId);

                b.Property(e => e.StaffId).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasOne(e => e.User).WithOne().HasForeignKey<Staff>(fk => fk.StaffId);
            });
        }

        static void CreateUser(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            //builder.Entity<Role>(b =>
            //{
            //    b.ToTable("Role");
            //    b.HasKey(e => e.RoleId);

            //    b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
            //    b.Property(e => e.Name).HasMaxLength(NameMaxLength).IsRequired();

            //    //b.HasMany(e => e.UserRoles)
            //    //    .WithOne(d => d.Role)
            //    //    .HasForeignKey(d => d.RoleId);
            //});
            //builder.Entity<User>(b =>
            //{
            //    b.ToTable("User");
            //    b.HasKey(e => e.UserId);

            //    b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
            //    b.Property(e => e.FirstName).HasMaxLength(NameMaxLength).IsRequired();
            //    b.Property(e => e.LastName).HasMaxLength(NameMaxLength).IsRequired();
            //    b.Property(e => e.Email).HasMaxLength(KeyMaxLength).IsRequired();
            //    b.Property(e => e.PhoneNumber).HasMaxLength(KeyMaxLength).IsRequired();

            //    b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

            //    b.HasMany(e => e.UserTasks)
            //        .WithOne(d => d.User)
            //        .HasForeignKey(d => d.UserId);

            //    //b.HasMany(a => a.GivenFeedbacks)
            //    //    .WithOne(j => j.GivenBy)
            //    //    .HasForeignKey(j => j.GivenById)
            //    //    //.OnDelete(DeleteBehavior.Restrict)
            //    //    ;

            //    //b.HasMany(a => a.ReceivedFeedbacks)
            //    //    .WithOne(j => j.ReceivedBy)
            //    //    .HasForeignKey(j => j.ReceivedById)
            //    //    //.OnDelete(DeleteBehavior.Restrict)
            //    //    ;
            //});
            //builder.Entity<UserRole>(b =>
            //{
            //    b.ToTable("UserRole");
            //    b.HasKey(e => new { e.UserId, e.RoleId });

            //    b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
            //    b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
            //});

            builder.Entity<UserTask>(b =>
            {
                b.ToTable("UserTask");
                b.HasKey(e => e.UserTaskId);

                b.Property(e => e.UserTaskId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.Title).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.Description).HasMaxLength(DescMaxLength);
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.UserTaskItems)
                    .WithOne(d => d.UserTask)
                    .HasForeignKey(d => d.UserTaskId);
            });

            builder.Entity<UserTaskItem>(b =>
            {
                b.ToTable("UserTaskItem");
                b.HasKey(e => e.UserTaskItemId);

                b.Property(e => e.UserTaskItemId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserTaskId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Title).HasMaxLength(DescMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();
            });
        }
    }
}
