using Microsoft.EntityFrameworkCore;
using testing.Models;

namespace testing.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(cfg =>
        {
            cfg.HasKey(x => x.Id);
            cfg.OwnsOne(x => x.Email, o =>
            {
                o.Property(p => p.Value).HasColumnName("Email").HasMaxLength(100).IsRequired();
                o.HasIndex(p => p.Value).IsUnique();
            });
            cfg.Property(x => x.Name).IsRequired().HasMaxLength(50);
            cfg.Property(x => x.Surname).IsRequired().HasMaxLength(50);
            cfg.Property(x => x.UserName).IsRequired().HasMaxLength(50);
            cfg.Property(x => x.Password).IsRequired().HasMaxLength(100);
        });

        b.Entity<RefreshToken>(cfg =>
        {
            cfg.ToTable("RefreshTokens");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.TokenHash).IsRequired().HasMaxLength(256);
            cfg.Property(x => x.CreatedAtUtc).IsRequired();
            cfg.Property(x => x.ExpiresAtUtc).IsRequired();
            cfg.HasIndex(x => x.TokenHash).IsUnique();
            cfg.HasIndex(x => x.UserId);
        });
    }
}