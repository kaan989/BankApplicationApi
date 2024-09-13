namespace BankApplication.Dto
{
    public class AccountApplicationDto
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public decimal MonthlyIncome { get; set; }
        public bool IsVadeli { get; set; } // Vadeli mi vadesiz mi?
        public bool Pending { get; set; }
        public bool IsApprovel { get; set; }

        // Relations
        public string AppUserId { get; set; } // Kullanıcı ID
    }
}
