using Billing.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.Repository.IRepository
{
    public interface IInvoiceRepository: IRepository<Invoice>
    {
        Task UpdateAsync(Invoice obj);
        Task SoftRemoveAsync(Invoice id);
    }
}
