using BankApplication.Models;
using System.Transactions;

namespace BankApplication.IRepository
{
    public interface ITransactionRepository
    {
        ICollection<TransactionMoney> GetAll();
        TransactionMoney GetbyId(int id);
        bool AddTransaction(TransactionMoney transaction, int accountId);
        bool DeleteTransaction(TransactionMoney transaction);
        bool Save();
    }
}
