﻿// <auto-generated />
using System;
using Data.Identity.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.migrations.identity
{
    [DbContext(typeof(IdentityWebContext))]
    [Migration("20210204034425_identity")]
    partial class identity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Data.Identity.Models.Feedback", b =>
                {
                    b.Property<string>("FeedbackId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("FeedbackCategory")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("FeedbackId");

                    b.HasIndex("UserId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Data.Identity.Models.LoginAudit", b =>
                {
                    b.Property<string>("LoginAuditId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LoginDate")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("RemoteIpAddress")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("LoginAuditId");

                    b.HasIndex("UserId");

                    b.ToTable("LoginAudit");
                });

            modelBuilder.Entity("Data.Identity.Models.Tenant", b =>
                {
                    b.Property<string>("TenantId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("DatabaseConnectionString")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<double>("GeoX")
                        .HasColumnType("REAL");

                    b.Property<double>("GeoY")
                        .HasColumnType("REAL");

                    b.Property<string>("Host")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("TenantId");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.IdentityWebUser", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT")
                        .HasColumnName("UserId");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("TenantId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("TenantId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.UserAddress", b =>
                {
                    b.Property<string>("UserAddressId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Default")
                        .HasColumnType("INTEGER");

                    b.Property<double>("GeoX")
                        .HasColumnType("REAL");

                    b.Property<double>("GeoY")
                        .HasColumnType("REAL");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserAddressId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAddress");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.UserInformation", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("UserInformation");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT")
                        .HasColumnName("RoleId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("Data.Identity.Models.Feedback", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Identity.Models.LoginAudit", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", "User")
                        .WithMany("LoginAudits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.IdentityWebUser", b =>
                {
                    b.HasOne("Data.Identity.Models.Tenant", "Tenant")
                        .WithMany("Users")
                        .HasForeignKey("TenantId");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.UserAddress", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.UserInformation", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", "User")
                        .WithOne("UserInformation")
                        .HasForeignKey("Data.Identity.Models.Users.UserInformation", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Data.Identity.Models.Users.IdentityWebUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Data.Identity.Models.Tenant", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Data.Identity.Models.Users.IdentityWebUser", b =>
                {
                    b.Navigation("LoginAudits");

                    b.Navigation("UserInformation");
                });
#pragma warning restore 612, 618
        }
    }
}
