using Billing.DataAccess.Authentication;
using Billing.Models.Authentication;
using Billing.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;
        private readonly ILogger _logger;

        public AuthenticateController(IAuthenticationServices authenticationServices, ILogger logger)
        {
            _authenticationServices = authenticationServices;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginViewModel>> LoginAsync([FromBody] Login model)
        {
            (bool isAuthenticated, JwtSecurityToken token) = await _authenticationServices.LoginAsync(model);
            if (isAuthenticated)
            {
                _logger.Information(@"{0} is authenticated", model.Username);

                try
                {
                    throw new Exception();
                }
                catch (Exception ex)
                {

                    _logger.Warning("{0} is authenticated", ex);
                }
                return Ok(new LoginViewModel()
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = model.Username
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] Register model)
        {
            var result = await _authenticationServices.RegisterAsync(model, UserRoles.User);
            if (result.Succeeded)
            {
                return Ok("User created successfully!");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors.FirstOrDefault().Description);
        }

        //[Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult> RegisterAdminAsync([FromBody] Register model)
        {
            var result = await _authenticationServices.RegisterAsync(model, UserRoles.Admin);
            if (result.Succeeded)
            {
                return Ok("User created successfully!");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors.FirstOrDefault().Description);
        }
    }
}
