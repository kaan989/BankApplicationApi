using BankApplication.Data.Enum;

namespace BankApplication.Models
{
    public class TransactionMoney
    {
        public int Id { get; set; }
        public decimal Amount { get; set; } // İşlem tutarı
        public DateTime TransactionDate { get; set; } // İşlem tarihi
        public TransactionType Type { get; set; } // Yatırma mı, çekme mi?

        // Relations
        public int AccountId { get; set; } // İşlemin yapıldığı hesap
        public Account Account { get; set; }
    }
}
