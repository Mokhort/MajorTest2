using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.EF
{
    public enum UserRole
    {
        Admin = 0,
        Moderator = 1,
        User = 2
    }

    public class Person
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public string Avatar { get; set; }
    }
}
