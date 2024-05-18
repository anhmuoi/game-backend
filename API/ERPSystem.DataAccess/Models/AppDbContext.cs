using System;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ERPSystem.DataAccess.Models;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("User ID=postgres;Password=postgres;Server=localhost;Port=5432;Database=duali_erp;Integrated Security=true;Pooling=true;");
            // optionsBuilder.UseNpgsql(ApplicationVariables.Configuration.GetConnectionString(Constants.Settings.DefaultConnection));
        }

        optionsBuilder.EnableSensitiveDataLogging(true);
    }
    [DbFunction("dec", "pdb")]
    public string Dec(string text1, string text2, string text3, int integer1)
    {
        throw new NotSupportedException();
    }
    [DbFunction("enc", "pdb")]
    public string Enc(string text1, string text2, string text3)
    {
        throw new NotSupportedException();
    }
    
    public DbSet<Account> Account { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Department> Department { get; set; }
    public DbSet<Setting> Setting { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<WorkSchedule> WorkSchedule { get; set; }
    public DbSet<WorkLog> WorkLog { get; set; }
    public DbSet<FolderLog> FolderLog { get; set; }
    public DbSet<MeetingLog> MeetingLog { get; set; }
    public DbSet<MeetingRoom> MeetingRoom { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<DailyReport> DailyReport { get; set; }
    public DbSet<PurchaseRecord> PurchaseRecord { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<UserFile> UserFile { get; set; }
    public DbSet<File> File { get; set; }
    public DbSet<Folder> Folder { get; set; }
    public DbSet<UserFolder> UserFolder { get; set; }
    public DbSet<MailTemplate> MailTemplate { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(AppDbContext).GetMethod(nameof(Dec), new[] { typeof(string), typeof(string), typeof(string), typeof(int) }));
        modelBuilder.HasDbFunction(typeof(AppDbContext).GetMethod(nameof(Enc), new[] { typeof(string), typeof(string), typeof(string) }));

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.UserName).IsRequired();
            entity.HasIndex(m => m.UserName).IsUnique();
            entity.Property(m => m.Password).IsRequired(false);
            
            entity.Property(m => m.Timezone).IsRequired(false);
            entity.Property(m => m.Language).IsRequired(false);
            entity.Property(m => m.RefreshToken).IsRequired(false);
            entity.HasOne(d => d.Role)
                .WithMany(p => p.Account)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });
        
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.HasIndex(m => m.Name).IsUnique();
            entity.Property(m => m.Description).IsRequired(false);
            entity.Property(m => m.Color).IsRequired(false);
            entity.Property(m => m.Type).IsRequired();
        });
        
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.HasIndex(m => m.Name).IsUnique();
            entity.Property(m => m.Number).IsRequired();
            entity.HasIndex(m => m.Number).IsUnique();
            
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.ChildDepartment)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Department_Tree");
            entity.HasOne(d => d.DepartmentManager)
                .WithMany(p => p.Department)
                .HasForeignKey(d => d.DepartmentManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_Account");
        });
        
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Key).IsRequired();
            entity.HasIndex(m => m.Key).IsUnique();
            entity.Property(m => m.Value).IsRequired();
            entity.HasIndex(m => m.Value).IsUnique();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.Property(m => m.Email).IsRequired(false);
            entity.Property(m => m.Phone).IsRequired(false);
            entity.Property(m => m.Position).IsRequired(false);
            entity.Property(m => m.Avatar).IsRequired(false);
            entity.Property(m => m.WalletAddress).IsRequired(false);
            
            entity.HasOne(d => d.Department)
                .WithMany(p => p.User)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Department");
            entity.HasOne(m => m.Account)
                .WithOne(m => m.User)
                .HasForeignKey<User>(m => m.AccountId)
                .IsRequired(false);
        });
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.HasIndex(m => m.Name).IsUnique();
            entity.Property(m => m.PermissionList).IsRequired(false);
            entity.Property(m => m.IsDefault).IsRequired();
        });
        modelBuilder.Entity<WorkSchedule>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Title).IsRequired();
            entity.Property(m => m.Content).IsRequired(false);
            entity.Property(m => m.CategoryId).IsRequired(false);
            entity.Property(m => m.FolderLogId).IsRequired(false);
            entity.Property(m => m.Type).IsRequired();
            entity.Property(m => m.StartDate).IsRequired();
            entity.Property(m => m.EndDate).IsRequired();
            entity.HasOne(m => m.Category)
                .WithMany(m => m.WorkSchedule)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkSchedule_Category");
            entity.HasOne(m => m.FolderLog)
                .WithMany(m => m.WorkSchedule)
                .HasForeignKey(m => m.FolderLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkSchedule_Folder");
        });
        modelBuilder.Entity<WorkLog>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Title).IsRequired();
            entity.Property(m => m.Content).IsRequired(false);
            entity.Property(m => m.StartDate).IsRequired();
            entity.Property(m => m.EndDate).IsRequired();
            entity.HasOne(m => m.User)
                .WithMany(m => m.WorkLog)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WorkLog_User");
            entity.HasOne(m => m.FolderLog)
                .WithMany(m => m.WorkLog)
                .HasForeignKey(m => m.FolderLogId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WorkLog_Folder");
        });
        modelBuilder.Entity<MeetingLog>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Title).IsRequired();
            entity.Property(m => m.Content).IsRequired(false);
            entity.Property(m => m.MeetingRoomId).IsRequired(false);
            entity.Property(m => m.StartDate).IsRequired(false);
            entity.Property(m => m.EndDate).IsRequired(false);
            entity.Property(m => m.UserList).IsRequired(false);

            entity.HasOne(m => m.MeetingRoom)
                .WithMany(m => m.MeetingLog)
                .HasForeignKey(m => m.MeetingRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeetingLog_MeetingRoom");
   
        });
        modelBuilder.Entity<DailyReport>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Title).IsRequired();
            entity.Property(m => m.Date).IsRequired();
            entity.Property(m => m.Content).IsRequired(false);
            entity.Property(m => m.ReporterId).IsRequired(false);
            entity.Property(m => m.FolderLogId).IsRequired(false);
            entity.HasOne(m => m.User)
                .WithMany(m => m.DailyReport)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DailyReport_User");
            entity.HasOne(m => m.Account)
                .WithMany(m => m.DailyReport)
                .HasForeignKey(m => m.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyReport_Reporter");
            entity.HasOne(m => m.FolderLog)
                .WithMany(m => m.DailyReport)
                .HasForeignKey(m => m.FolderLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyReport_FolderLog");
        });
        modelBuilder.Entity<FolderLog>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.Property(m => m.Description).IsRequired(false);
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.ChildFolderLog)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_FolderLog_Tree");
        });
        modelBuilder.Entity<MeetingRoom>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.HasIndex(m => m.Name).IsUnique();
            entity.Property(m => m.Description).IsRequired(false);
            entity.Property(m => m.UserListId).IsRequired(false);
        });
        modelBuilder.Entity<PurchaseRecord>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Title).IsRequired();
            entity.HasIndex(m => m.Title).IsUnique();
            entity.Property(m => m.Note).IsRequired(false);
            entity.Property(m => m.Quantity).IsRequired();
            entity.Property(m => m.Status).IsRequired();
            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.PurchaseRecord)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PurchaseRecord_Supplier");
        });
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.MainProduct).IsRequired();
            entity.Property(m => m.Name).IsRequired();
            entity.HasIndex(m => m.Name).IsUnique();
            entity.Property(m => m.Remark).IsRequired(false);
            entity.Property(m => m.Phone).IsRequired(false);
            entity.Property(m => m.Address).IsRequired(false);
            entity.Property(m => m.BusinessType).IsRequired(false);
        });
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.Property(m => m.Description).IsRequired(false);
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.ChildFolder)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Folder_Tree");
        });
        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
            
            entity.Property(m => m.Name).IsRequired();
            entity.Property(m => m.Description).IsRequired(false);
            entity.HasOne(d => d.Folder)
                .WithMany(p => p.File)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_File_Folder");
        });
        modelBuilder.Entity<UserFile>(entity =>
        {
            entity.HasKey(m => new { m.UserId, m.FileId });
                
            entity.Property(m => m.PermissionType).IsRequired();
            entity.HasOne(d => d.User)
                .WithMany(p => p.UserFile)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserFile_User");
            entity.HasOne(d => d.File)
                .WithMany(p => p.UserFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserFile_File");
        });
        modelBuilder.Entity<UserFolder>(entity =>
        {
            entity.HasKey(m => new { m.UserId, m.FolderId });
                
            entity.Property(m => m.PermissionType).IsRequired();
            entity.HasOne(d => d.User)
                .WithMany(p => p.UserFolder)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserFolder_User");
            entity.HasOne(d => d.Folder)
                .WithMany(p => p.UserFolder)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserFolder_File");
        });
        modelBuilder.Entity<MailTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.IsEnable).HasDefaultValue(true);
            });
    }
}