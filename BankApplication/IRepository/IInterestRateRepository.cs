using BankApplication.Models;

namespace BankApplication.IRepository
{
    public interface IInterestRateRepository
    {
        Task<IEnumerable<InterestRate>> GetAllRatesAsync();
        Task<InterestRate> GetRateByIdAsync(int id);
        Task<bool> AddRateAsync(InterestRate rate);
        Task<bool> UpdateRateAsync(InterestRate rate);
        Task<bool> DeleteRateAsync(int id);
        Task<bool> SaveAsync();
    }
}
