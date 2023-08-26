using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_1.Dto;
using RESTful_1.Enumeration;
using RESTful_1.Filter;
using RESTful_1.Models;
using RESTful_1.Wrappers;
using System.Text.Json;

namespace RESTful_1.Controller
{
    [Route("api/")]
    [ApiController]
    public class ATransactionsController : ControllerBase
    {
        private readonly ATransactionContext _dbContext;

        public ATransactionsController(ATransactionContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET - for testing purpose
        [HttpGet("transactionsAll")]
        public async Task<ActionResult<IEnumerable<ATransaction>>> GetATransactionsAll()
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            return await _dbContext.ATransactionSet.ToListAsync();    
        }

        // GET
        [HttpGet("transactions")]
        public async Task<ActionResult> GetATransactions([FromQuery] PaginationFilter filter) //FilterData from body
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _dbContext.ATransactionSet
                .Select(p => new ATransactionResponseDto() { PaymentId = p.PaymentId, Amount = p.Amount, Currency = p.Currency, 
                                                                CardholderNumber = p.CardholderNumber.Substring(0, 6) + "******" +
                                                                    p.CardholderNumber.Substring(p.CardholderNumber.Length - 4), 
                                                                HolderName = p.HolderName, 
                                                                OrderReference = p.OrderReference, Status = p.Status.ToString(),
                                                                CreatedDate = p.CreatedDateTime
                })
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            if (pagedData == null)
            {
                return NotFound();

            }
            var totalRecords = await _dbContext.ATransactionSet.CountAsync();
            return Ok(new PagedResponse<List<ATransactionResponseDto>>(pagedData, validFilter.PageNumber, validFilter.PageSize, totalRecords));
        }

        // GET api/authorize/{id}
        [HttpGet("authorize/{id}")]
        public async Task<ActionResult<ATransaction>> GetATransaction(string id) 
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            var aTransaction = await _dbContext.ATransactionSet.FindAsync(id);

            if (aTransaction == null) {
                return NotFound();
            }

            return aTransaction;
        }

        // POST api/authorize/
        [HttpPost("authorize")]
        public async Task<ActionResult<ATransaction>> PostAuthorize(ATransaction aTransaction)
        {
            aTransaction.PaymentId = Guid.NewGuid();
            aTransaction.CreatedDateTime = DateTime.Now;
            _dbContext.ATransactionSet.Add(aTransaction);
            await _dbContext.SaveChangesAsync();

            var authorizeResponse = new AuthorizeResponseDto(aTransaction.PaymentId, StatusType.Authorized.ToString());

            return CreatedAtAction(nameof(GetATransaction), new { id = aTransaction.Id }, authorizeResponse);
        }

        // POST api/authorize/{id}/voids
        [HttpPost("authorize/{id}/voids")]
        public async Task<ActionResult<ATransaction>> PostAuthorizeVoids(Guid id, [FromBody] JsonElement body)
        {
            string orderReference = System.Text.Json.JsonSerializer.Serialize(body);

            var entityToUpdate = await _dbContext.ATransactionSet.FirstOrDefaultAsync(s => s.PaymentId == id);
            if (entityToUpdate == null)
            {
                return NotFound();
            }
            entityToUpdate.Status = StatusType.Voided;
            try
            {
                await _dbContext.SaveChangesAsync();
                var authorizeResponse = new AuthorizeResponseDto(entityToUpdate.PaymentId, entityToUpdate.Status.ToString());
                return CreatedAtAction(nameof(GetATransaction), new { id = entityToUpdate.Id }, authorizeResponse);
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return Problem();
            
        }

        // POST api/authorize/{id}/capture
        [HttpPost("authorize/{id}/capture")]
        public async Task<ActionResult<ATransaction>> PostAuthorizeCapture(Guid id, [FromBody] JsonElement body)
        {
            string orderReference = System.Text.Json.JsonSerializer.Serialize(body);

            var entityToUpdate = await _dbContext.ATransactionSet.FirstOrDefaultAsync(s => s.PaymentId == id);
            if (entityToUpdate == null)
            {
                return NotFound();
            }
            entityToUpdate.Status = StatusType.Captured;
            try
            {
                await _dbContext.SaveChangesAsync();
                var authorizeResponse = new AuthorizeResponseDto(entityToUpdate.PaymentId, entityToUpdate.Status.ToString());
                return CreatedAtAction(nameof(GetATransaction), new { id = entityToUpdate.Id }, authorizeResponse);
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return Problem();

        }
    }
}
