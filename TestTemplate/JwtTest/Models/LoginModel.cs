using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Имя пользователя является обязательным")]
        [DisplayName("Имя пользователя")]
        public string Username { get; set; }
        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Пароль является обязательным")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
