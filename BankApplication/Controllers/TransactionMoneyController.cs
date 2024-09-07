using BankApplication.Dto;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // GET: api/Transaction
        [HttpGet]
        public ActionResult<IEnumerable<TransactionMoney>> GetAllTransactions()
        {
            var transactions = _transactionRepository.GetAll();
            return Ok(transactions);
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public ActionResult<TransactionMoney> GetTransactionById(int id)
        {
            var transaction = _transactionRepository.GetbyId(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        // POST: api/Transaction
        [HttpPost]
        public IActionResult AddTransaction([FromBody] TransactionMoneyDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest(ModelState);
            }

            var transaciton = new TransactionMoney
            {
                Id = transactionDto.Id,
                Amount = transactionDto.Amount,
                TransactionDate = transactionDto.TransactionDate,
                Type = transactionDto.Type,
                AccountId = transactionDto.AccountId,
            };

            if(!_transactionRepository.AddTransaction(transaciton, transactionDto.AccountId))
            {
                ModelState.AddModelError("", "Account creation failed.");
                return StatusCode(500, ModelState);
            }

            return Ok(transaciton);
        }

        // DELETE: api/Transaction/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            var transaction = _transactionRepository.GetbyId(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var success = _transactionRepository.DeleteTransaction(transaction);
            if (!success)
            {
                return BadRequest("Failed to delete transaction");
            }

            return Ok();
        }
    }
}