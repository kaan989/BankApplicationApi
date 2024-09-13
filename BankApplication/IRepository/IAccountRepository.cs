using BankApplication.Models;

namespace BankApplication.IRepository
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountByNumberAsync(string accountNumber);
        ICollection<Account> GetAllAccounts();
        ICollection<Account> GetAllAccountsByUserId(string userId);
        Task<Account> GetById(int id);
        bool AddAcount(Account account, string AppUserId); 
        bool UpdateAcount(Account account);
        bool DeleteAccount(Account account);
        bool Save();
    }
}
