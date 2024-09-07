using BankApplication.Data;
using BankApplication.IRepository;
using BankApplication.Models;

namespace BankApplication.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly ApplicationDbContext _context;
        public AppUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool DeleteProfile(AppUser user)
        {
           _context.Users.Remove(user);
            return Save();
        }

        public ICollection<AppUser> GetAll()
        {
            return _context.Users.OrderBy(x => x.Id).ToList();
        }

        public  AppUser GetById(string id)
        {
            return  _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
