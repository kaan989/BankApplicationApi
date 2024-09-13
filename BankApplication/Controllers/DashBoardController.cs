using BankApplication.Data;
using BankApplication.Data.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var userCount = await _context.Users.CountAsync();
            var totalBalance = await _context.Accounts.SumAsync(a => a.Balance);
            var fixedAccounts = await _context.Accounts.CountAsync(a => a.Type == AccountType.vadeli);

            // Vadesiz hesap sayısı
            var currentAccounts = await _context.Accounts.CountAsync(a => a.Type == AccountType.vadesiz);
            var approvedApplications = await _context.Applications.CountAsync(a => a.IsApprovel);
            var rejectedApplications = await _context.Applications.CountAsync(a => !a.IsApprovel && !a.Pending);

            var result = new
            {
                ApprovedApplications = approvedApplications,
                RejectedApplications = rejectedApplications,
                UserCount = userCount,
                TotalBalance = totalBalance,
                FixedAccounts = fixedAccounts,
                CurrentAccounts = currentAccounts
            };

            return Ok(result);
        }
    }
}