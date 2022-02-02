using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Models.ViewModels
{
    public class LoginViewModel
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public string username { get; set; }
    }
}
