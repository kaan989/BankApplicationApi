using BankApplication.Data;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool AddAcount(Account account,string AppUserId)
        {
            var user = _context.Users.Find(AppUserId);
            if(user == null) 
                return false;

            account.AppUserId = AppUserId;
            account.AppUser = user;

            _context.Accounts.Add(account);
            return Save();
        }

        public bool DeleteAccount(Account account)
        {
            _context.Accounts.Remove(account);
            return Save();
        }

        public Task<Account> GetAccountByNumberAsync(string accountNumber)
        {
            return _context.Accounts
                .Include(a => a.AppUser).FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public ICollection<Account> GetAllAccounts()
        {
            return _context.Accounts.Include(a => a.AppUser).OrderBy(a=> a.Id).ToList();
        }

        public Task<Account> GetById(int id)
        {
            return _context.Accounts.Include(a =>a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAcount(Account account)
        {
           _context.Update(account);
            return Save();
        }
    }
}
