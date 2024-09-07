using System.ComponentModel.DataAnnotations;

namespace BankApplication.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string IdNumber { get; set; } // Kimlik numarası
        public string FirstName { get; set; } // Kullanıcı adı
        public string LastName { get; set; } // Kullanıcı soyadı
        public string Address { get; set; } // Kullanıcı adresi
    }
}
