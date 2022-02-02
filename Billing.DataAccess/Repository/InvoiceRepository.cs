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
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        ApplicationDbContext _db;
        public InvoiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task SoftRemoveAsync(Invoice obj)
        {
            Invoice objFromDb = await _db.Invoices.FirstOrDefaultAsync(p => p.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Status = StatusEnum.Deleted;
                objFromDb.TimeStamp = DateTime.Now;
            }
        }

        public async Task UpdateAsync(Invoice obj)
        {
            var objFromDb = await _db.Invoices.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.CustomerId = obj.CustomerId;
                objFromDb.Amount = obj.Amount;
                objFromDb.DeadLine = obj.DeadLine;
                objFromDb.TimeStamp = DateTime.Now;
            }
        }
    }
}
