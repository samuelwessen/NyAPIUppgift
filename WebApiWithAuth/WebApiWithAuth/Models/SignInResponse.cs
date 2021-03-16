using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithAuth.Models
{
    public class SignInResponse
    {
        public bool Succeded { get; set; }
        public SignInResponseResult Result { get; set; }
    }

    public class SignInResponseResult
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
    }
}
