using BankApplication.Data;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Repository
{
    public class AccountApplicationRepository : IAccountApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool AddAccountApplication(AccountApplication accountApplication, string appUserId)
        {
            var user = _context.Users.Find(appUserId);
            if (user == null)
            {
                return false;
            }

            accountApplication.AppUser = user;
            accountApplication.AppUserId = appUserId;
            
            _context.Applications.Add(accountApplication);
            return Save();  


        }

        public bool DeleteAccountApplication(AccountApplication accountApplication)
        {
            _context.Applications.Remove(accountApplication);
            return Save();
                
        }

     

        public ICollection<AccountApplication> GetAll()
        {
            return _context.Applications.Include(a => a.AppUser).OrderBy(a => a.Id).ToList();   
        }

        public ICollection<AccountApplication> GetAllAplicationByUserId(string userId)
        {
           return _context.Applications.Include(a => a.AppUser).Where(a => a.AppUserId == userId).ToList();
        }

        public async Task<AccountApplication> GetById(int id)
        {
            var application = await _context.Applications
                                            .Include(a => a.AppUser)
                                            .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                throw new NullReferenceException($"No AccountApplication found with Id: {id}");
            }

            return application;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;    
        }

        public bool UpdateAccountApplication(AccountApplication accountApplication)
        {
            _context.Update(accountApplication);
            return Save();
        }
    }
}
