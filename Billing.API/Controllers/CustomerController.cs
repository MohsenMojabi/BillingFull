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
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public CustomerController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }


        // GET: api/<CustomerController>
        [HttpGet("GetAll")]
        [HttpGet("GetAll/{searchedTerm}")]
        //[HttpGet]
        //[HttpGet("{searchedTerm}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<string>>> Get(string searchedTerm = "")
        {
            ///
            /// If returning from cache isn't successfull or the returned value is null
            /// then read from the database and store in cache
            ///
            if (_cache.TryGetValue(CacheKeysEnum.GET_ALL_CUSTOMERS, out IEnumerable<Customer> result) 
                || result == null || !result.Any())
            {
                result = await _unitOfWork.Customer.GetAllAsync();
                _cache.Set(CacheKeysEnum.GET_ALL_CUSTOMERS, result);
            }

            result = result.Where(p => p.FirstName.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase) || p.LastName.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase) || (p.Email != null && p.Email.Contains(searchedTerm, StringComparison.OrdinalIgnoreCase))).ToList();

            return Ok(result);
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Customer>> Get(Guid id)
        {
            var result = await _unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "InvoiceList");
            if (result == null)
            {
                return NotFound("Customer does not exist");
            }
            return Ok(result);
        }

        // POST api/<CustomerController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] Customer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var objFromDb = _unitOfWork.Customer.GetFirstOrDefaultAsync(u => u.Id == model.Id).Result;
            if (objFromDb != null)
            {
                return BadRequest("Customer exists in database");
            }
            await _unitOfWork.Customer.AddAsync(model);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_CUSTOMERS);
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);

            return Ok("Customer created successfully");
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] Customer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var objFromDb = _unitOfWork.Customer.GetFirstOrDefaultAsync(u => u.Id == id).Result;
            if (objFromDb == null)
            {
                return NotFound("Customer does not exist");
            }
            model.Id = id;
            await _unitOfWork.Customer.UpdateAsync(model);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_CUSTOMERS);
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);

            return Ok("Customer updated successfully");
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var objFromDb = await _unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "InvoiceList");
            if (objFromDb == null)
            {
                return NotFound("Customer does not exist");
            }
            if (objFromDb.InvoiceList != null && objFromDb.InvoiceList.Count > 0)
            {
                return BadRequest("This customer has some invoices. First delete them");
            }
            await _unitOfWork.Customer.SoftRemoveAsync(objFromDb);
            await _unitOfWork.SaveAsync();
            _cache.Remove(CacheKeysEnum.GET_ALL_CUSTOMERS);
            _cache.Remove(CacheKeysEnum.GET_ALL_INVOICES);
            return Ok("Customer deleted successfully");
        }
    }
}
