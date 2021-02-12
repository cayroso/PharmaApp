
using Data.App.Models.Appointments;
using Data.App.Models.Calendars;
using Data.App.Models.Chats;
using Data.App.Models.Clinics;
using Data.App.Models.FileUploads;
using Data.App.Models.Parents;
using Data.App.Models.Users;
using Data.Identity.Models;
using Data.Identity.Models.Users;
using Data.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
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


        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatReceiver> ChatReceivers { get; set; }


        public DbSet<FileUpload> FileUploads { get; set; }

        public DbSet<Clinic> Pharmacies { get; set; }

        public DbSet<User> Users { get; set; }
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

            CreateAppointments(builder);

            CreateCalendar(builder);

            CreateChats(builder);

            CreateClinics(builder);

            CreateFileUploads(builder);

            CreateParents(builder);

            CreateUser(builder);
        }

        void CreateAppointments(ModelBuilder builder)
        {
            builder.Entity<Appointment>(b =>
            {
                b.ToTable("Appointment");
                b.HasKey(e => e.AppointmentId);

                b.Property(e => e.AppointmentId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ClinicId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChildId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.MedicalEntries)
                    .WithOne(d => d.Appointment)
                    .HasForeignKey(f => f.AppointmentId);
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

        void CreateClinics(ModelBuilder builder)
        {
            builder.Entity<Clinic>(b =>
            {
                b.ToTable("Clinic");
                b.HasKey(e => e.ClinicId);

                b.Property(e => e.ClinicId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Appointments)
                    .WithOne(d => d.Clinic)
                    .HasForeignKey(f => f.ClinicId);

                b.HasMany(e => e.Parents)
                    .WithOne(d => d.Clinic)
                    .HasForeignKey(f => f.ClinicId);

                b.HasMany(e => e.Staffs)
                    .WithOne(d => d.Clinic)
                    .HasForeignKey(f => f.ClinicId);
            });

            builder.Entity<ClinicParent>(b =>
            {
                b.ToTable("ClinicParent");
                b.HasKey(e => e.ClinicParentId);

                b.Property(e => e.ClinicParentId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ClinicId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ParentId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<ClinicReview>(b =>
            {
                b.ToTable("ClinicReview");
                b.HasKey(e => e.ClinicReviewId);

                b.Property(e => e.ClinicReviewId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ClinicId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ParentId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<ClinicStaff>(b =>
            {
                b.ToTable("ClinicStaff");
                b.HasKey(e => e.ClinicStaffId);

                b.Property(e => e.ClinicStaffId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ClinicId).HasMaxLength(KeyMaxLength).IsRequired();
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

        void CreateParents(ModelBuilder builder)
        {
            builder.Entity<Parent>(b =>
            {
                b.ToTable("Parent");
                b.HasKey(e => e.ParentId);

                b.Property(e => e.ParentId).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasOne(e => e.User).WithOne().HasForeignKey<Parent>(fk => fk.ParentId);

                b.HasMany(e => e.Children)
                    .WithOne(d => d.Parent)
                    .HasForeignKey(f => f.ParentId);

                b.HasMany(e => e.Clinics)
                    .WithOne(d => d.Parent)
                    .HasForeignKey(f => f.ParentId);
            });

            builder.Entity<Child>(b =>
            {
                b.ToTable("Child");
                b.HasKey(e => e.ChildId);

                b.Property(e => e.ChildId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ParentId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ImageId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.MedicalEntries)
                    .WithOne(d => d.Child)
                    .HasForeignKey(f => f.ChildId);

                b.HasMany(e => e.Appointments)
                    .WithOne(d => d.Child)
                    .HasForeignKey(f => f.ChildId);
            });

            builder.Entity<ChildMedicalEntry>(b =>
            {
                b.ToTable("ChildMedicalEntry");
                b.HasKey(e => e.ChildMedicalEntryId);

                b.Property(e => e.ChildMedicalEntryId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.AppointmentId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChildId).HasMaxLength(KeyMaxLength).IsRequired();                
            });
        }

        static void CreateUser(ModelBuilder builder)
        {

            builder.Entity<Role>(b =>
            {
                b.ToTable("Role");
                b.HasKey(e => e.RoleId);

                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Name).HasMaxLength(NameMaxLength).IsRequired();

                //b.HasMany(e => e.UserRoles)
                //    .WithOne(d => d.Role)
                //    .HasForeignKey(d => d.RoleId);
            });
            builder.Entity<User>(b =>
            {
                b.ToTable("User");
                b.HasKey(e => e.UserId);

                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.FirstName).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.LastName).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.Email).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PhoneNumber).HasMaxLength(KeyMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.UserTasks)
                    .WithOne(d => d.User)
                    .HasForeignKey(d => d.UserId);

                //b.HasMany(a => a.GivenFeedbacks)
                //    .WithOne(j => j.GivenBy)
                //    .HasForeignKey(j => j.GivenById)
                //    //.OnDelete(DeleteBehavior.Restrict)
                //    ;

                //b.HasMany(a => a.ReceivedFeedbacks)
                //    .WithOne(j => j.ReceivedBy)
                //    .HasForeignKey(j => j.ReceivedById)
                //    //.OnDelete(DeleteBehavior.Restrict)
                //    ;
            });
            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRole");
                b.HasKey(e => new { e.UserId, e.RoleId });

                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
            });
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
