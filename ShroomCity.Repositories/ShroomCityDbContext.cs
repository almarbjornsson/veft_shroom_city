using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using ShroomCity.Repositories.Seed;
using Attribute = ShroomCity.Models.Entities.Attribute;

namespace ShroomCity.Repositories
{
    public class ShroomCityDbContext : DbContext
    {
        public ShroomCityDbContext(DbContextOptions<ShroomCityDbContext> options) : base(options)
        {
        }

        public DbSet<Mushroom> Mushrooms { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data
            modelBuilder.Seed();

            // Define many-to-many relationships

            modelBuilder.Entity<Mushroom>()
                .HasMany(m => m.Attributes)
                .WithMany(a => a.Mushrooms);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles);

            
            
        }
    }
}