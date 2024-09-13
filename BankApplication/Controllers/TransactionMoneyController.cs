using BankApplication.Data.Enum;
using BankApplication.Dto;
using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

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

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferDto transferDto)
        {
            if (transferDto == null || transferDto.Amount <= 0)
            {
                return BadRequest("Invalid transfer details.");
            }

            using (var scope = new TransactionScope())
            {
                // Çekim yapılan hesap
                var fromAccount = _transactionRepository.GetAccountById(transferDto.FromAccountId);
                if (fromAccount == null || fromAccount.Balance < transferDto.Amount)
                {
                    return BadRequest("Insufficient balance or account not found.");
                }

                // Yatırma yapılan hesap
                var toAccount = _transactionRepository.GetAccountByNumber(transferDto.ToAccountNumber);
                if (toAccount == null)
                {
                    return BadRequest("Destination account not found.");
                }

                // Çekim işlemi (Withdraw)
                var withdrawTransaction = new TransactionMoney
                {
                    Amount = transferDto.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Type = TransactionType.Withdraw,
                    AccountId = transferDto.FromAccountId
                };
                

                // Yatırma işlemi (Deposit)
                var depositTransaction = new TransactionMoney
                {
                    Amount = transferDto.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Type = TransactionType.Deposit,
                    AccountId = toAccount.Id
                };
                

                try
                {
                    // Veritabanına işlemleri kaydet
                    if (!_transactionRepository.AddTransaction(withdrawTransaction, fromAccount.Id) ||
                        !_transactionRepository.AddTransaction(depositTransaction, toAccount.Id))
                    {
                        throw new Exception("Error processing transfer.");
                    }

                    // Eğer her şey yolundaysa transaction'ı tamamla
                    scope.Complete();

                    return Ok(new { Message = "Transfer successful", FromAccount = fromAccount, ToAccount = toAccount });
                }
                catch (Exception ex)
                {
                    // Hata durumunda tüm işlemleri geri al
                    return StatusCode(500, "Transfer failed: " + ex.Message);
                }
            }
        }
    }


}