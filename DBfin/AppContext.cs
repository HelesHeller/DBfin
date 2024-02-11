using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Transactions;

namespace DBfin
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaulConnection"))
                .LogTo(output => Debug.WriteLine(output), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<Settings>(s => s.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId);

            Seed(modelBuilder);
        }
        private void Seed(ModelBuilder modelBuilder)
        {
            if (Database.GetPendingMigrations().Any())
                return;

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Food" },
                new Category { CategoryId = 2, CategoryName = "Transport" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, UserName = "Nick" },
                new User { UserId = 2, UserName = "Sasha" }
            );

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    TransactionId = 1,
                    Amount = 100,
                    UserId = 1,
                    CategoryId = 1,
                    TransactionDate = new DateTime(2024, 2, 9)
                },
                new Transaction
                {
                    TransactionId = 2,
                    Amount = 50,
                    UserId = 1,
                    CategoryId = 2,
                    TransactionDate = new DateTime(2024, 2, 10)
                }
            );

            modelBuilder.Entity<Settings>().HasData(
                new Settings { SettingsId = 1, Country = "Ukraine", UserId = 1 },
                new Settings { SettingsId = 2, Country = "USA", UserId = 2 }
            );
        }
    }
}
