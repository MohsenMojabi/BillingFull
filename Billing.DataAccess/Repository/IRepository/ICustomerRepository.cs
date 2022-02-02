using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billing.Models.Models;

namespace Billing.DataAccess.Repository.IRepository
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task UpdateAsync(Customer obj);
        Task SoftRemoveAsync(Customer obj);
    }
}
