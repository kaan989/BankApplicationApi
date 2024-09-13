using AutoMapper;
using BankApplication.Data;
using BankApplication.Data.Enum;
using BankApplication.Dto;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AccountController(ApplicationDbContext context, IMapper mapper, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Account>))]
        public IActionResult GetAccounts()
        {
            var accounts = _mapper.Map<List<AccountDto>>(_accountRepository.GetAllAccounts());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(accounts);
        }



        [HttpGet("user/{userid}")]
        [ProducesResponseType(200, Type = typeof(AccountDto))]
        [ProducesResponseType(400)]
        public IActionResult GetAllAcountByUserId(string userid)
        {
            var accounts = _mapper.Map<List<AccountDto>>(_accountRepository.GetAllAccountsByUserId(userid));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(accounts);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(AccountDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _accountRepository.GetById(id);
            if (account == null)
            {
                return NotFound();
            }

            var accountDto = _mapper.Map<AccountDto>(account);
            return Ok(accountDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult PostAccount([FromBody] AccountDto accountDto)
        {
            if (accountDto == null)
                return BadRequest(ModelState);

            // IBAN otomatik olarak oluşturuluyor
            var accountNumber = IBANGenerator.GenerateIBAN();

            // Vadeli hesaplar için endDate nullable
            var account = new Account
            {
                AccountNumber = accountNumber, // Otomatik oluşturulan IBAN
                Balance = accountDto.Balance,
                Type = accountDto.Type,
                CreatedAt = DateTime.Now,
                LastInterestAppliedDate = accountDto.LastInterestAppliedDate,
                AppUserId = accountDto.AppUserId
            };

            if(_context.Accounts.Any(a => a.AccountNumber == account.AccountNumber)){
                ModelState.AddModelError("", "This iban is not unqie");
                return BadRequest(ModelState);
            }
            if (!_accountRepository.AddAcount(account, accountDto.AppUserId))
            {
                ModelState.AddModelError("", "Account creation failed.");
                return StatusCode(500, ModelState);
            }

            return Ok(account);
        }


        [HttpDelete("{AccountId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAccount(int AccountId)
        {
            var accountToDelete = await _accountRepository.GetById(AccountId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(accountToDelete == null)
            {
                return NotFound();  
            }
            if (!_accountRepository.DeleteAccount(accountToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
            }

            return Ok();
        }


    }

    public static class IBANGenerator
    {
        private static Random random = new Random();

        public static string GenerateIBAN()
        {

            // Ülke kodu ve kontrol rakamı sabit
            string countryCode = "TR";
            string checkDigits = "00"; // Kontrol rakamı genelde banka tarafından hesaplanır
            string bankCode = "22"; // Örnek olarak bankaya özel bir kod (gerçek bankalarda farklı olabilir)

            // 16 haneli rastgele hesap numarası üret
            string accountNumber = GenerateRandomNumber(16);

            // IBAN formatında oluştur (TR + kontrol rakamı + banka kodu + hesap numarası)
            return $"{countryCode}{checkDigits}{bankCode}{accountNumber}";
        }

        // Belirli bir uzunlukta rastgele sayı üret
        private static string GenerateRandomNumber(int length)
        {
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = (char)('0' + random.Next(10));
            }
            return new string(result);
        }
    }



}
