using Billing.DataAccess.Repository.IRepository;
using Billing.Models.Authentication;
using Billing.Models.Models;
using Billing.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Billing.API.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public InvoiceController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }


        // GET: api/<InvoiceController>
        [HttpGet("GetAll")]
        [HttpGet("GetAll/{searchedTerm}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Invoice>>> Get(string searchedTerm = "")
        {
            ///
            /// If returning from cache isn't successfull or the returned value is null 
            /// then read from the database and store in cache
            ///
            if (_cache.TryGetValue(CacheKeysEnum.GET_ALL_INVOICES, out IEnumerable<Invoice> result)
                || result == null || !result.Any())
            {
                result = await _unitOfWork.Invoice.GetAllAsync(includeProperties: "Customer");
                _cache.Set(CacheKeysEnum.GET_ALL_INVOICES, result);
            }

            result = result.Where(p => p.Customer.FirstName.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase) || p.Customer.LastName.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase) || (p.Customer != null && p.Customer.Email != null && p.Customer.Email.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase))).ToList();

            return Ok(result);
        }

        // GET api/<InvoiceController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> Get(Guid id)
        {
            var result = await _unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "Customer");
            if (result == null)
            {
                return NotFound("Invoice does not exist");
            }
            return Ok(result);
        }

        // POST api/<InvoiceController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> Post([FromBody] Invoice model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var objFromDb = _unitOfWork.Invoice.GetFirstOrDefaultAsync(u => u.Id == model.Id).Result;
            if (objFromDb != null)
            {
                return BadRequest("Invoice exists in database");
            }
            var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.Id == model.CustomerId);
            if (customer == null)
            {
                return NotFound("Customer does not exist");
            }
            await _unitOfWork.Invoice.AddAsync(model);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);
            return Ok("Invoice created successfully");
        }

        // PUT api/<InvoiceController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] Invoice model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var objFromDb = await _unitOfWork.Invoice.GetFirstOrDefaultAsync(u => u.Id == id);
            if (objFromDb == null)
            {
                return NotFound("Invoice does not exist");
            }
            var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.Id == model.CustomerId);
            if (customer == null)
            {
                return NotFound("Customer does not exist");
            }
            model.Id = id;
            await _unitOfWork.Invoice.UpdateAsync(model);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);
            return Ok("Invoice updated successfully");
        }

        // DELETE api/<InvoiceController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var objFromDb = await _unitOfWork.Invoice.GetFirstOrDefaultAsync(u => u.Id == id);
            if (objFromDb == null)
            {
                return NotFound("Invoice does not exist");
            }
            await _unitOfWork.Invoice.SoftRemoveAsync(objFromDb);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);
            return Ok("Invoice deleted successfully");
        }
    }
}
