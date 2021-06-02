using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя пользователя является обязательным")]
        [DisplayName("Имя пользователя")]
        public string Username { get; set; }
        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Пароль является обязательным")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage ="Пароль должен быть не менее 8 символов, содержать 1 цифру и 1 заглавную и прописную букву")]
        public string Password { get; set; }
        [DisplayName("Потдвердите Пароль")]
        [Required(ErrorMessage = "Подтверждение пароля является обязательным")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Пароль должен быть не менее 8 символов, содержать 1 цифру и 1 заглавную и прописную букву")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Загрузите Аватар")]
        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; }
    }
}
