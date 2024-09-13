using BankApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<InterestRate> InterestRates { get; set; }
        public DbSet<TransactionMoney> Transactions { get; set; }
        public DbSet<AccountApplication> Applications { get; set; }


    }
}
