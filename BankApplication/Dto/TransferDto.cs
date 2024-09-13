namespace BankApplication.Dto
{
    public class TransferDto
    {
        public int FromAccountId { get; set; } // Çekim yapılan hesap
        public string ToAccountNumber { get; set; }   // Para yatırılan hesap
        public decimal Amount { get; set; }    // Transfer miktarı
    }
}
