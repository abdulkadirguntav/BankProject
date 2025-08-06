using BankProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BankProject2.Data
{
    internal class BankDbContext : DbContext
    {
        public DbSet<Transactions> transactions { get; set; }
        public DbSet<Accounts> accounts { get; set; }
        public DbSet<Customer> customer { get; set; }
        public DbSet<Currency> currency { get; set; }
        public DbSet<Loan> loan { get; set; }
        public DbSet<LoanPayment> loanApplication { get; set; }
        public DbSet<CreditCard> creditCard { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=bank;Uid=root;Pwd=Retro1320;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasKey(c => c.CurrencyID);

            modelBuilder.Entity<Currency>()
                .Property(c => c.CurrencyID)
                .ValueGeneratedOnAdd();
        }
    }
}
 