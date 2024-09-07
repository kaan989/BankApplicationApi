namespace BankApplication.Models
{
    public class InterestRate
    {
        public int Id { get; set; }
        public decimal Rate { get; set; } // Faiz oranı (%)

        public DateTime EffectiveFrom { get; set; } // Faiz oranının geçerli olduğu tarih
        public DateTime? EffectiveTo { get; set; } // Faiz oranının sona erdiği tarih
    }
}
