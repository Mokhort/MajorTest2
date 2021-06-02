using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Models
{
    public class MyPage
    {
        public string Username { get; set; }
        public IFormFile Avatar { get; set; }

        public string Gift { get; set; }

    }
}
