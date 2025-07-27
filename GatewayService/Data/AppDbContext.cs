using GatewayService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GatewayService.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // User & Role management
    // Token tracking
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role-Permission join key
        modelBuilder.Entity<AccessToken>()
            .HasIndex(t => t.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.Token)
            .IsUnique();
    }
}