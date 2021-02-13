﻿// <auto-generated />
using System;
using Data.App.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.migrations.app
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Data.App.Models.Brands.Brand", b =>
                {
                    b.Property<string>("BrandId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("BrandId");

                    b.HasIndex("PharmacyId");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("Data.App.Models.Calendars.Calendar", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Day")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DayName")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DayOfYear")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MonthName")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quarter")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Date");

                    b.ToTable("Calendar");
                });

            modelBuilder.Entity("Data.App.Models.Chats.Chat", b =>
                {
                    b.Property<string>("ChatId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatId");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatMessage", b =>
                {
                    b.Property<string>("ChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("ChatMessageType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateSent")
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatMessageId");

                    b.HasIndex("ChatId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatReceiver", b =>
                {
                    b.Property<string>("ChatReceiverId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ReceiverId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatReceiverId");

                    b.HasIndex("ChatId");

                    b.HasIndex("ReceiverId");

                    b.ToTable("ChatReceiver");
                });

            modelBuilder.Entity("Data.App.Models.Customers.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("CustomerId");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.Drug", b =>
                {
                    b.Property<string>("DrugId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BrandId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Classification")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<double>("ReorderLevel")
                        .HasColumnType("REAL");

                    b.Property<double>("SafetyStock")
                        .HasColumnType("REAL");

                    b.Property<double>("Stock")
                        .HasColumnType("REAL");

                    b.HasKey("DrugId");

                    b.HasIndex("BrandId");

                    b.HasIndex("PharmacyId");

                    b.ToTable("Drug");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.DrugPrice", b =>
                {
                    b.Property<string>("DrugPriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Cogs")
                        .HasColumnType("REAL");

                    b.Property<string>("DrugId")
                        .HasColumnType("TEXT");

                    b.Property<uint>("LoyaltyPoints")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("SaleEnd")
                        .HasColumnType("TEXT");

                    b.Property<double>("SalePrice")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("SaleStart")
                        .HasColumnType("TEXT");

                    b.HasKey("DrugPriceId");

                    b.HasIndex("DrugId");

                    b.ToTable("DrugPrice");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.DrugSubscription", b =>
                {
                    b.Property<string>("DrugSubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DrugId")
                        .HasColumnType("TEXT");

                    b.HasKey("DrugSubscriptionId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DrugId");

                    b.ToTable("DrugSubscription");
                });

            modelBuilder.Entity("Data.App.Models.FileUploads.FileUpload", b =>
                {
                    b.Property<string>("FileUploadId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("ContentDisposition")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContentType")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<long>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.HasKey("FileUploadId");

                    b.ToTable("FileUpload");
                });

            modelBuilder.Entity("Data.App.Models.Orders.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("CancelReason")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndPickupDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("GrossPrice")
                        .HasColumnType("REAL");

                    b.Property<string>("Number")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PharmacyId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartPickupDate")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("PharmacyId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Data.App.Models.Orders.OrderLineItems.OrderLineItem", b =>
                {
                    b.Property<string>("OrderLineItemId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("DrugId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("DrugPriceId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<double>("ExtendedPrice")
                        .HasColumnType("REAL");

                    b.Property<string>("LineNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<double>("Quantity")
                        .HasColumnType("REAL");

                    b.HasKey("OrderLineItemId");

                    b.HasIndex("DrugId");

                    b.HasIndex("DrugPriceId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderLineItem");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.Pharmacy", b =>
                {
                    b.Property<string>("PharmacyId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<double>("GeoX")
                        .HasColumnType("REAL");

                    b.Property<double>("GeoY")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpeningHours")
                        .HasColumnType("TEXT");

                    b.Property<int>("PharmacyStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("PharmacyId");

                    b.ToTable("Pharmacy");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.PharmacyReview", b =>
                {
                    b.Property<string>("PharmacyReviewId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.HasKey("PharmacyReviewId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("PharmacyId");

                    b.ToTable("PharmacyReview");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.PharmacyStaff", b =>
                {
                    b.Property<string>("PharmacyStaffId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("StaffId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("PharmacyStaffId");

                    b.HasIndex("PharmacyId");

                    b.HasIndex("StaffId");

                    b.ToTable("PharmacyStaff");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.Staff", b =>
                {
                    b.Property<string>("StaffId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("StaffId");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("Data.App.Models.Users.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasColumnType("TEXT");

                    b.Property<double>("OverallRating")
                        .HasColumnType("REAL");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<double>("TotalRating")
                        .HasColumnType("REAL");

                    b.HasKey("UserId");

                    b.HasIndex("ImageId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserRole", b =>
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

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.Property<string>("UserTaskId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateActualCompleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateAssigned")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserTaskId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTask");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTaskItem", b =>
                {
                    b.Property<string>("UserTaskItemId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDone")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserTaskId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserTaskItemId");

                    b.HasIndex("UserTaskId");

                    b.ToTable("UserTaskItem");
                });

            modelBuilder.Entity("Data.App.Models.Brands.Brand", b =>
                {
                    b.HasOne("Data.App.Models.Pharmacies.Pharmacy", "Pharmacy")
                        .WithMany("Brands")
                        .HasForeignKey("PharmacyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pharmacy");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatMessage", b =>
                {
                    b.HasOne("Data.App.Models.Chats.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Chat");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatReceiver", b =>
                {
                    b.HasOne("Data.App.Models.Chats.Chat", "Chat")
                        .WithMany("Receivers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Receiver");
                });

            modelBuilder.Entity("Data.App.Models.Customers.Customer", b =>
                {
                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Data.App.Models.Customers.Customer", "CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.Drug", b =>
                {
                    b.HasOne("Data.App.Models.Brands.Brand", "Brand")
                        .WithMany("Drugs")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Pharmacies.Pharmacy", "Pharmacy")
                        .WithMany("Drugs")
                        .HasForeignKey("PharmacyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Pharmacy");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.DrugPrice", b =>
                {
                    b.HasOne("Data.App.Models.Drugs.Drug", "Drug")
                        .WithMany("Prices")
                        .HasForeignKey("DrugId");

                    b.Navigation("Drug");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.DrugSubscription", b =>
                {
                    b.HasOne("Data.App.Models.Customers.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.HasOne("Data.App.Models.Drugs.Drug", "Drug")
                        .WithMany("CustomerSubscriptions")
                        .HasForeignKey("DrugId");

                    b.Navigation("Customer");

                    b.Navigation("Drug");
                });

            modelBuilder.Entity("Data.App.Models.Orders.Order", b =>
                {
                    b.HasOne("Data.App.Models.Customers.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Pharmacies.Pharmacy", "Pharmacy")
                        .WithMany("Orders")
                        .HasForeignKey("PharmacyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Pharmacy");
                });

            modelBuilder.Entity("Data.App.Models.Orders.OrderLineItems.OrderLineItem", b =>
                {
                    b.HasOne("Data.App.Models.Drugs.Drug", "Drug")
                        .WithMany("OrderLineItems")
                        .HasForeignKey("DrugId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Drugs.DrugPrice", "DrugPrice")
                        .WithMany("OrderLineItems")
                        .HasForeignKey("DrugPriceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Orders.Order", "Order")
                        .WithMany("LineItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Drug");

                    b.Navigation("DrugPrice");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.PharmacyReview", b =>
                {
                    b.HasOne("Data.App.Models.Customers.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Pharmacies.Pharmacy", "Pharmacy")
                        .WithMany("Reviews")
                        .HasForeignKey("PharmacyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Pharmacy");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.PharmacyStaff", b =>
                {
                    b.HasOne("Data.App.Models.Pharmacies.Pharmacy", "Pharmacy")
                        .WithMany("Staffs")
                        .HasForeignKey("PharmacyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Pharmacies.Staff", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pharmacy");

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.Staff", b =>
                {
                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Data.App.Models.Pharmacies.Staff", "StaffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.HasOne("Data.App.Models.FileUploads.FileUpload", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserRole", b =>
                {
                    b.HasOne("Data.App.Models.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.HasOne("Data.App.Models.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithMany("UserTasks")
                        .HasForeignKey("UserId");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTaskItem", b =>
                {
                    b.HasOne("Data.App.Models.Users.UserTask", "UserTask")
                        .WithMany("UserTaskItems")
                        .HasForeignKey("UserTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserTask");
                });

            modelBuilder.Entity("Data.App.Models.Brands.Brand", b =>
                {
                    b.Navigation("Drugs");
                });

            modelBuilder.Entity("Data.App.Models.Chats.Chat", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Receivers");
                });

            modelBuilder.Entity("Data.App.Models.Customers.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.Drug", b =>
                {
                    b.Navigation("CustomerSubscriptions");

                    b.Navigation("OrderLineItems");

                    b.Navigation("Prices");
                });

            modelBuilder.Entity("Data.App.Models.Drugs.DrugPrice", b =>
                {
                    b.Navigation("OrderLineItems");
                });

            modelBuilder.Entity("Data.App.Models.Orders.Order", b =>
                {
                    b.Navigation("LineItems");
                });

            modelBuilder.Entity("Data.App.Models.Pharmacies.Pharmacy", b =>
                {
                    b.Navigation("Brands");

                    b.Navigation("Drugs");

                    b.Navigation("Orders");

                    b.Navigation("Reviews");

                    b.Navigation("Staffs");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.Navigation("UserRoles");

                    b.Navigation("UserTasks");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.Navigation("UserTaskItems");
                });
#pragma warning restore 612, 618
        }
    }
}
