
using Microsoft.EntityFrameworkCore;
using SteamInventoryBoosterV2.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
