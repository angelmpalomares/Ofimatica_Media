using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class InfraDbContext : DbContext
    {
        public InfraDbContext(DbContextOptions<InfraDbContext> options) : base(options)
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ResourceModel> Resources { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Username)
                    .IsUnique();
                entity.HasIndex(e => e.Email)
                    .IsUnique();
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                entity.Property(e => e.LoginRetries)
                    .HasDefaultValue(0);
            });
            modelBuilder.Entity<ResourceModel>(entity =>
            {
                entity.ToTable("Resources");
                entity.HasKey(e => e.ResourceId);
                entity.Property(e => e.ResourceId)
                    .ValueGeneratedOnAdd();
                entity.HasIndex(e => e.ResourceType);
                entity.Property(e => e.Name)
                    .HasMaxLength(100);
                entity.Property(e => e.Description)
                    .HasMaxLength(3000);
                entity.Property(e => e.Author)
                    .HasMaxLength(50);
            });
        }
    }
}
                