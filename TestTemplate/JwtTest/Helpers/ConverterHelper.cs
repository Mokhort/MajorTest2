using Isopoh.Cryptography.Argon2;
using JwtTest.EF;
using JwtTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Helpers
{
    public static class ConverterHelper
    {
        public static Person ToPerson(this UserModel model)
        {
            return new Person()
            {
                Login = model.Username,
                PasswordHash = Argon2.Hash(model.Password),
                Role = model.Role,
                Id = model.Id.HasValue?model.Id.Value:0
            };
        }

        public static UserModel ToUserModel(this Person person)
        {
            return new UserModel()
            {
                Id = person.Id,
                Role = person.Role,
                Username = person.Login
            };
        }

        public static EditUserModel ToEditUserModel(this Person person)
        {
            return new EditUserModel()
            {
                Id = person.Id,
                Role = person.Role,
                Username = person.Login
            };
        }


    }
}
