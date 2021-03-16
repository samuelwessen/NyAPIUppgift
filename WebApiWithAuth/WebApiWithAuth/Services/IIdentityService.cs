using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApiWithAuth.Models;

namespace WebApiWithAuth.Services
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(SignUp model);
        Task<bool> CreateErrandAsync(CreateErrandViewModel model);
        Task<SignInResponse> SignInAsync(string email, string password);

        Task<IEnumerable<Errand>> Search(string status);

        Task<IEnumerable<UserResponse>> GetUsersAsync();

        bool ValidateAccessRights(RequestUser requestUser);

    }
}
