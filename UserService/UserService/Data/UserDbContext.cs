using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Security;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Config relationship
            // user-role
            modelBuilder.Entity<User>(e => e.HasIndex(ex=>ex.Email).IsUnique());

            modelBuilder.Entity<User>()
                .HasOne(u => u.Roles)
                .WithMany(r => r.Users)
                .HasForeignKey(r => r.RoleId);
            // user-permission
            modelBuilder.Entity<UserPermission>()
                .HasOne(u => u.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(r => r.UserId);
            modelBuilder.Entity<UserPermission>()
                .HasOne(p => p.Permission)
                .WithMany(u => u.Permissions)
                .HasForeignKey(r => r.PermissionId);
           


        }
    }
}
