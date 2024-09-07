using Microsoft.AspNetCore.Identity;

namespace BankApplication.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string Address { get; set; }
    }
}
