using BankApplication.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BankApplication.IRepository
{
    public interface IAppUserRepository
    {
        ICollection<AppUser> GetAll();
        AppUser GetById(string id);
        bool DeleteProfile(AppUser user);
        bool Save();
    }
}
