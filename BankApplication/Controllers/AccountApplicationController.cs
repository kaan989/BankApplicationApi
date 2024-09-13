using AutoMapper;
using BankApplication.Data;
using BankApplication.Dto;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApplicationController : Controller
    {
        private readonly IAccountApplicationRepository _accountAppRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AccountApplicationController(ApplicationDbContext context, IMapper mapper, IAccountApplicationRepository accountAppRepository)
        {
            _accountAppRepository = accountAppRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AccountApplication>))]
        public IActionResult GetAll()
        {
            var accountsapp = _mapper.Map<List<AccountApplicationDto>>(_accountAppRepository.GetAll());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(accountsapp);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(AccountApplicationDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAccountById( int id)
        {
            var application = await _accountAppRepository.GetById(id);
            if(application == null)
            {
                return NotFound();
            }

            var applicationDto = _mapper.Map<AccountApplicationDto>(application);
            return Ok(applicationDto);
        }

        [HttpGet("aplication/{id}")]
        [ProducesResponseType(200, Type = typeof(AccountApplicationDto))]
        [ProducesResponseType(400)]
        public IActionResult GetApplicationByUserId(string id)
        {
            var aplications = _mapper.Map<List<AccountApplicationDto>>(_accountAppRepository.GetAllAplicationByUserId(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(aplications);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult PostApplication([FromBody] AccountApplicationDto accountApplicationDto)
        {
            if(accountApplicationDto == null)
            {
                return NotFound(ModelState);
            }

            var isApprovel = false;
            var pending = true;

            var accountsApplication = new AccountApplication
            {
                Age = accountApplicationDto.Age,
                IsApprovel = isApprovel,
                Pending = pending,
                AppUserId = accountApplicationDto.AppUserId,
                MonthlyIncome = accountApplicationDto.MonthlyIncome,
                IsVadeli = accountApplicationDto.IsVadeli,
            };

            if(!_accountAppRepository.AddAccountApplication(accountsApplication, accountApplicationDto.AppUserId))
            {
                ModelState.AddModelError("", "Account creation failed.");
                return StatusCode(500, ModelState);
            }

            return Ok(accountsApplication);
        }

        [HttpDelete("{AccountappId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAccount(int AccountappId)
        {
            var deletedApp = await _accountAppRepository.GetById(AccountappId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (deletedApp == null)
            {
                return NotFound();
            }
           


            if (!_accountAppRepository.DeleteAccountApplication(deletedApp))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
            }

            return Ok();
        }

        [HttpPut("{accountappId}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        public IActionResult UpdateAccountApplication(int accountappId, [FromBody] AccountApplicationDto updatedAccountApplicationDto)
        {
           if(accountappId != updatedAccountApplicationDto.Id)
            {
                return BadRequest(ModelState);
            }

            // Güncellenecek başvuruyu veritabanından buluyoruz.
            var existingApplication = _accountAppRepository.GetById(accountappId).Result;

            if (existingApplication == null)
            {
                return NotFound();
            }

            existingApplication.IsApprovel = updatedAccountApplicationDto.IsApprovel;
            existingApplication.Pending = updatedAccountApplicationDto.Pending;

            // Güncelleme işlemini gerçekleştiriyoruz
            if (!_accountAppRepository.UpdateAccountApplication(existingApplication))
            {
                ModelState.AddModelError("", "Account update failed.");
                return StatusCode(500, ModelState);
            }

            return Ok(); // 204
        }

    }
}
