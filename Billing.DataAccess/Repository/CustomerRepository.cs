using Billing.DataAccess.Data;
using Billing.DataAccess.Repository.IRepository;
using Billing.Models.Models;
using Billing.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        ApplicationDbContext _db;
        public CustomerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task SoftRemoveAsync(Customer obj)
        {
            Customer objFromDb = await _db.Customers.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Status = StatusEnum.Deleted;
                objFromDb.TimeStamp = DateTime.Now;
            }
        }

        public async Task UpdateAsync(Customer obj)
        {
            var objFromDb = await _db.Customers.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.FirstName = obj.FirstName;
                objFromDb.LastName = obj.LastName;
                objFromDb.Email = obj.Email;
                objFromDb.ImgUrl = obj.ImgUrl;
                objFromDb.TimeStamp = DateTime.Now;
            }
        }
    }
}
