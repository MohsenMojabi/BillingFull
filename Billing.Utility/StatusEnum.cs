using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Utility
{
    public enum StatusEnum: byte
    {
        Deleted = 0,
        Pending = 1,
        Approved = 2
    }
}
