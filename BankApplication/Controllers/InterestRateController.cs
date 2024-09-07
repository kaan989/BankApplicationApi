using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterestRateController : ControllerBase
    {
        private readonly IInterestRateRepository _repository;

        public InterestRateController(IInterestRateRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRates()
        {
            var rates = await _repository.GetAllRatesAsync();
            return Ok(rates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRateById(int id)
        {
            var rate = await _repository.GetRateByIdAsync(id);
            if (rate == null) return NotFound();
            return Ok(rate);
        }

        [HttpPost]
        public async Task<IActionResult> AddRate([FromBody] InterestRate rate)
        {
            if (rate == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.AddRateAsync(rate);
            if (!result) return StatusCode(500, "A problem occurred while handling your request.");

            return CreatedAtAction(nameof(GetRateById), new { id = rate.Id }, rate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRate(int id, [FromBody] InterestRate rate)
        {
            if (id != rate.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingRate = await _repository.GetRateByIdAsync(id);
            if (existingRate == null) return NotFound();

            existingRate.Rate = rate.Rate;
            existingRate.EffectiveFrom = rate.EffectiveFrom;
            existingRate.EffectiveTo = rate.EffectiveTo;

            var result = await _repository.UpdateRateAsync(existingRate);
            if (!result) return StatusCode(500, "A problem occurred while handling your request.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRate(int id)
        {
            var result = await _repository.DeleteRateAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
