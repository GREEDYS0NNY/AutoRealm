using AutoRealmProject.Backend.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoRealmProject.Backend.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CarAd> CarAds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CarAd>()
                .Property(c => c.CarPhoto)
                .HasColumnType("varbinary(max)");

            base.OnModelCreating(builder);
        }
    }
}
