using Microsoft.EntityFrameworkCore;
using UserManagementService.Entities;

namespace UserManagementService.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleRolePermission> RoleRolePermissions => Set<RoleRolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User-Role relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);

        // RolePermission hierarchy
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Parent)
            .WithMany(rp => rp.Children)
            .HasForeignKey(rp => rp.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Role-RolePermission many-to-many
        modelBuilder.Entity<RoleRolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.RolePermissionId });

        modelBuilder.Entity<RoleRolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RoleRolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RoleRolePermission>()
            .HasOne(rp => rp.RolePermission)
            .WithMany(rp => rp.RoleRolePermissions)
            .HasForeignKey(rp => rp.RolePermissionId);
    }
}
