
using Microsoft.EntityFrameworkCore;
using SteamInventoryBoosterV2.Models.Domain;

namespace SteamInventoryBoosterV2.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SteamItem> SteamItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SteamItem>()
                .HasIndex(u => u.Link)
                .IsUnique();

            modelBuilder.Entity<SteamItem>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<OwnedItem>()
                .HasOne(o => o.SteamItem)
                .WithMany()
                .HasForeignKey(o => o.ClassId);
        }
    }
}
