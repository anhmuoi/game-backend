﻿// <auto-generated />
using System;
using ERPSystem.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERPSystem.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240318101258_updateFolderFile")]
    partial class updateFolderFile
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDateRefreshToken")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<string>("Timezone")
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Account");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.DailyReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FolderLogId")
                        .HasColumnType("integer");

                    b.Property<int>("ReporterId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FolderLogId");

                    b.HasIndex("ReporterId");

                    b.HasIndex("UserId");

                    b.ToTable("DailyReport");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("DepartmentManagerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentManagerId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Number")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("FolderId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("File");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.FolderLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("FolderLog");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.MeetingLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FolderLogId")
                        .HasColumnType("integer");

                    b.Property<int>("MeetingRoomId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FolderLogId");

                    b.HasIndex("MeetingRoomId");

                    b.ToTable("MeetingLog");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.MeetingRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MeetingRoom");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.PurchaseRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int?>("SupplierId")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("PurchaseRecord");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PermissionList")
                        .HasColumnType("text");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("BusinessType")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MainProduct")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Supplier");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .HasColumnType("text");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.HasIndex("DepartmentId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.UserFile", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PermissionType")
                        .HasColumnType("integer");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("UserFile");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.UserFolder", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("FolderId")
                        .HasColumnType("integer");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PermissionType")
                        .HasColumnType("integer");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "FolderId");

                    b.HasIndex("FolderId");

                    b.ToTable("UserFolder");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.WorkLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FolderLogId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FolderLogId");

                    b.HasIndex("UserId");

                    b.ToTable("WorkLog");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.WorkSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FolderLogId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("FolderLogId");

                    b.ToTable("WorkSchedule");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Account", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Role", "Role")
                        .WithMany("Account")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_Account_Role");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.DailyReport", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.FolderLog", "FolderLog")
                        .WithMany("DailyReport")
                        .HasForeignKey("FolderLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DailyReport_FolderLog");

                    b.HasOne("ERPSystem.DataAccess.Models.Account", "Account")
                        .WithMany("DailyReport")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DailyReport_Reporter");

                    b.HasOne("ERPSystem.DataAccess.Models.User", "User")
                        .WithMany("DailyReport")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DailyReport_User");

                    b.Navigation("Account");

                    b.Navigation("FolderLog");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Department", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Account", "DepartmentManager")
                        .WithMany("Department")
                        .HasForeignKey("DepartmentManagerId")
                        .HasConstraintName("FK_Department_Account");

                    b.HasOne("ERPSystem.DataAccess.Models.Department", "Parent")
                        .WithMany("ChildDepartment")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Department_Tree");

                    b.Navigation("DepartmentManager");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.File", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Folder", "Folder")
                        .WithMany("File")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_File_Folder");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Folder", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Folder", "Parent")
                        .WithMany("ChildFolder")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Folder_Tree");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.FolderLog", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.FolderLog", "Parent")
                        .WithMany("ChildFolderLog")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_FolderLog_Tree");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.MeetingLog", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.FolderLog", "FolderLog")
                        .WithMany("MeetingLog")
                        .HasForeignKey("FolderLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MeetingLog_FolderLog");

                    b.HasOne("ERPSystem.DataAccess.Models.MeetingRoom", "MeetingRoom")
                        .WithMany("MeetingLog")
                        .HasForeignKey("MeetingRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MeetingLog_MeetingRoom");

                    b.Navigation("FolderLog");

                    b.Navigation("MeetingRoom");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.PurchaseRecord", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Supplier", "Supplier")
                        .WithMany("PurchaseRecord")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseRecord_Supplier");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.User", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Account", "Account")
                        .WithOne("User")
                        .HasForeignKey("ERPSystem.DataAccess.Models.User", "AccountId");

                    b.HasOne("ERPSystem.DataAccess.Models.Department", "Department")
                        .WithMany("User")
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("FK_User_Department");

                    b.Navigation("Account");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.UserFile", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.File", "File")
                        .WithMany("UserFile")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserFile_File");

                    b.HasOne("ERPSystem.DataAccess.Models.User", "User")
                        .WithMany("UserFile")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserFile_User");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.UserFolder", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Folder", "Folder")
                        .WithMany("UserFolder")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserFolder_File");

                    b.HasOne("ERPSystem.DataAccess.Models.User", "User")
                        .WithMany("UserFolder")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserFolder_User");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.WorkLog", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.FolderLog", "FolderLog")
                        .WithMany("WorkLog")
                        .HasForeignKey("FolderLogId")
                        .IsRequired()
                        .HasConstraintName("FK_WorkLog_Folder");

                    b.HasOne("ERPSystem.DataAccess.Models.User", "User")
                        .WithMany("WorkLog")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_WorkLog_User");

                    b.Navigation("FolderLog");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.WorkSchedule", b =>
                {
                    b.HasOne("ERPSystem.DataAccess.Models.Category", "Category")
                        .WithMany("WorkSchedule")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_WorkSchedule_Category");

                    b.HasOne("ERPSystem.DataAccess.Models.FolderLog", "FolderLog")
                        .WithMany("WorkSchedule")
                        .HasForeignKey("FolderLogId")
                        .IsRequired()
                        .HasConstraintName("FK_WorkSchedule_Folder");

                    b.Navigation("Category");

                    b.Navigation("FolderLog");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Account", b =>
                {
                    b.Navigation("DailyReport");

                    b.Navigation("Department");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Category", b =>
                {
                    b.Navigation("WorkSchedule");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Department", b =>
                {
                    b.Navigation("ChildDepartment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.File", b =>
                {
                    b.Navigation("UserFile");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Folder", b =>
                {
                    b.Navigation("ChildFolder");

                    b.Navigation("File");

                    b.Navigation("UserFolder");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.FolderLog", b =>
                {
                    b.Navigation("ChildFolderLog");

                    b.Navigation("DailyReport");

                    b.Navigation("MeetingLog");

                    b.Navigation("WorkLog");

                    b.Navigation("WorkSchedule");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.MeetingRoom", b =>
                {
                    b.Navigation("MeetingLog");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Role", b =>
                {
                    b.Navigation("Account");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.Supplier", b =>
                {
                    b.Navigation("PurchaseRecord");
                });

            modelBuilder.Entity("ERPSystem.DataAccess.Models.User", b =>
                {
                    b.Navigation("DailyReport");

                    b.Navigation("UserFile");

                    b.Navigation("UserFolder");

                    b.Navigation("WorkLog");
                });
#pragma warning restore 612, 618
        }
    }
}
