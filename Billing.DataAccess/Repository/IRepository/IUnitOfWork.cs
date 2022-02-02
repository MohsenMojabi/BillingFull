using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICustomerRepository Customer { get; }
        public IInvoiceRepository Invoice { get; }
        Task SaveAsync();
    }
}
