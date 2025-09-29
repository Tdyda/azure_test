using Microsoft.EntityFrameworkCore;
using testing.Models;

namespace testing.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

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
    }
}