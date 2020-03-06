using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Models
{
    /// <summary>
    /// Context базы данных.
    /// </summary>
    public class WalletApiContext : DbContext
    {
        public WalletApiContext(DbContextOptions<WalletApiContext> options) : base(options)
        {
            //Создание базы данных, если ее нет
            Database.EnsureCreated();
            //Инициализация БД
            DBInitializerdotNetWallet.Seed(this); 
        }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Unique Email User
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            //Unique Code Currency
            modelBuilder.Entity<Currency>()
            .HasIndex(u => u.Code)
            .IsUnique();
            //составной ключ
            modelBuilder.Entity<Wallet>().HasKey(u => new { u.UserId, u.CurrencyId});
        }

    }
}
