using BankApplication.Models;
using System.Transactions;

namespace BankApplication.IRepository
{
    public interface ITransactionRepository
    {
        ICollection<TransactionMoney> GetAll();
        TransactionMoney GetbyId(int id);
        Account GetAccountById(int accountId);
        Account GetAccountByNumber(string accountNumber);

        bool AddTransaction(TransactionMoney transaction, int accountId);
        bool DeleteTransaction(TransactionMoney transaction);
        bool Save();
    }
}
