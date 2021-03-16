using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiWithAuth.Data;
using WebApiWithAuth.Models;
using WebApiWithAuth.Services;

namespace WebApiWithAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {       
        private readonly IIdentityService _identity;

        public UsersController(IIdentityService identity)
        {            
            _identity = identity;
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUp model)
        {
            if (await _identity.CreateUserAsync(model))
                return new OkResult();

            return new BadRequestResult();
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignIn model)
        {
            var response = await _identity.SignInAsync(model.Email, model.Password);

            if (response.Succeded)
                return new OkObjectResult(response.Result);

            return new BadRequestResult();
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if(_identity.ValidateAccessRights(IdentityRequest()))
                return new OkObjectResult(await _identity.GetUsersAsync());

            return new UnauthorizedResult();
        }


        private RequestUser IdentityRequest()
        {            
            return new RequestUser
            {
                UserId = int.Parse(HttpContext.User.FindFirst("UserId").Value),
                AccessToken = Request.Headers["Authorization"].ToString().Split(" ")[1]
            };            
        }

    }
}
