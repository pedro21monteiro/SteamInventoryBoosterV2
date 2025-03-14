
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbname = "SteamInventoryBoosterDbV2";
            var dbhost = ".\\SQLEXPRESS";
            optionsBuilder.UseSqlServer($"Server={dbhost};Database={dbname};Trusted_Connection=True;TrustServerCertificate=True;");
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
