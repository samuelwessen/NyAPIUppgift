using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApiWithAuth.Data;
using WebApiWithAuth.Models;

namespace WebApiWithAuth.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SqlDbContext _context;
        private IConfiguration _configuration { get; }


        public IdentityService(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> CreateUserAsync(SignUp model)
        {
            if (!_context.Users.Any(user => user.Email == model.Email))
            {
                try
                {
                    var user = new User()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    };
                    user.CreatePasswordWithHash(model.Password);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch
                {

                }
            }

            return false;
        }

        public async Task<SignInResponse> SignInAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

                if (user != null)
                {
                    try
                    {
                        if (user.ValidatePasswordHash(password))
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var _secretKey = Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value);

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(new Claim[] { new Claim("UserId", user.Id.ToString()) }),
                                Expires = DateTime.Now.AddHours(5),
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey), SecurityAlgorithms.HmacSha512Signature)
                            };

                            var _accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

                            _context.SessionTokens.Add(new SessionToken { UserId = user.Id, AccessToken = _accessToken });
                            await _context.SaveChangesAsync();

                            return new SignInResponse 
                            {
                                Succeded = true,
                                Result = new SignInResponseResult
                                {
                                    Id = user.Id,
                                    Email = user.Email,
                                    AccessToken = _accessToken
                                }                                
                            };
                        }                        
                    }
                    catch { }
                }
            }
            catch { }         

            return new SignInResponse { Succeded = false };            
            
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = new List<UserResponse>();

            foreach (var user in await _context.Users.ToListAsync())
            {
                users.Add(new UserResponse { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
            }

            return users;
        }

        public bool ValidateAccessRights(RequestUser requestUser)
        {
            if (_context.SessionTokens.Any(x => x.UserId == requestUser.UserId && x.AccessToken == requestUser.AccessToken))
                return true;

            return false;
        }

        public async Task<bool> CreateErrandAsync(CreateErrandViewModel model)
        {
            try
            {
                var errand = new Errand()
                {
                    CustomerName = model.CustomerName,
                    UserId = model.UserId,
                    Created = model.Created,                    
                    Status = model.Status,
                    Description = model.Description
                };
                _context.Errands.Add(errand);
                await _context.SaveChangesAsync();

                return true;
            }
            catch { }

            return false;
        }


        //för att kunna söka på STATUS
        public async Task<IEnumerable<Errand>> SearchStatusAsync(string status)
        {
            IQueryable<Errand> query = _context.Errands;

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status.Contains(status));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Errand>> SearchCustomerAsync(string customername)
        {
            IQueryable<Errand> query = _context.Errands;

            if (!string.IsNullOrEmpty(customername))
            {
                query = query.Where(e => e.CustomerName.Contains(customername));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Errand>> SearchCreatedDateAsync(string createddate)
        {
            IQueryable<Errand> result = _context.Errands;


            if (DateTime.TryParse(createddate, out DateTime pdatetime))
            {
                result = result.Where(x => x.Created > pdatetime);
            }
            else if (createddate == "latest")
            {
                result = result.OrderByDescending(x => x.Created);
            }

            return await result.ToListAsync();
        }
    }
}
