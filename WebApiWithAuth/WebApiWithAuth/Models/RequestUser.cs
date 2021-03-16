using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithAuth.Models
{
    public class RequestUser
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
