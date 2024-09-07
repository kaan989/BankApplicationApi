using BankApplication.Data.Enum;

namespace BankApplication.Dto
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } // Hesap Numarası
        public decimal Balance { get; set; } // Hesap Bakiyesi
        public AccountType Type { get; set; } // Vadeli mi, Vadesiz mi?
        public DateTime CreatedAt { get; set; } // Hesabın oluşturulma tarihi
        public DateTime? LastInterestAppliedDate { get; set; } // Vadeli hesaba faiz uygulama tarihi (eğer varsa)
        public string AppUserId { get; set; } // Hesap Sahibi Kullanıcı ID'si
    }
}
