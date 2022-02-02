using Billing.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.Authentication
{
    public interface IAuthenticationServices
    {
        Task<(bool, JwtSecurityToken?)> LoginAsync(Login login);
        Task<IdentityResult> RegisterAsync(Register register, string role);
    }
}
