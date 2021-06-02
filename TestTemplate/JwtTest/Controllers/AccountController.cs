using JwtTest.EF;
using JwtTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtTest.Helpers;
using Microsoft.AspNetCore.Identity;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace JwtTest.Controllers
{
    public class AccountController : BaseController
    {


        public AccountController(JwtContext context, IOptions<AuthOptions> options, IHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.options = options;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: options.Value.Issuer,
                    audience: options.Value.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(options.Value.Lifetime)),
                    signingCredentials: new SigningCredentials(options.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            Person person = context.People.SingleOrDefault(x => x.Login == username);
            if (person != null && Argon2.Verify(person.PasswordHash, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, Enum.GetName(person.Role))
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        private async Task<bool> RegisterUser(string username, string password, UserRole role, IFormFile file)
        {
            if (context.People.Any(p => p.Login == username))
                return false;
            string randomFile = null;
            if (file != null)
            {
                randomFile = $"{Path.GetRandomFileName()}.{Path.GetExtension(file.FileName)}";

            }
            Person person = new Person()
            {
                Login = username,
                PasswordHash = Argon2.Hash(password),
                Role = role,
                Avatar = randomFile
            };
            await context.People.AddAsync(person);
            await context.SaveChangesAsync();
            if (file != null)
            {
                person = context.Entry(person).Entity;
                string userPath = Path.Combine(ImageFolder, person.Id.ToString());
                if (!Directory.Exists(userPath))
                    Directory.CreateDirectory(userPath);
                await file.WriteToFile(Path.Combine(userPath, randomFile));
            }
            return true;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await RegisterUser(model.Username, model.Password, UserRole.User, model.Avatar))
                return Redirect("/Home/Index");
            else
            {
                ModelState.AddModelError("Username", "Данное имя уже используется");
                return (View(model));
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            Person person = context.People.SingleOrDefault(usr => usr.Login == model.Username);
            if (person == null || !Argon2.Verify(person.PasswordHash, model.Password))
            {
                ModelState.AddModelError("Username", "Неверное имя пользователя или пароль");
                return View(model);
            }
            await Authenticate(person.Login, person.Role);
            return Redirect("/Home/Index");
        }

        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            await Logout();
            return Redirect("/Home/Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(UserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await RegisterUser(model.Username, model.Password, model.Role, model.Avatar))
                return Redirect("/Home/Index");
            else
            {
                ModelState.AddModelError("Username", "Данное имя уже используется");
                return (View(model));
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            return View(context.People);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(int id)
        {
            Person person = context.People.Find(id);
            return View(person.ToEditUserModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            Person person = context.People.Find(model.Id);
            if (person != null)
            {
                bool taken = person.Login != model.Username && context.People.Any(p => p.Login == model.Username);
                if (taken)
                {
                    ModelState.AddModelError("Username", "Данное имя уже занято");
                    return (View(model));
                }
                if (model.Avatar != null)
                {
                    string userDir = Path.Combine(ImageFolder, person.Id.ToString());
                    if (person.Avatar != null)
                        System.IO.File.Delete(Path.Combine(userDir, person.Avatar));
                    else if (!Directory.Exists(userDir))
                        Directory.CreateDirectory(userDir);
                    person.Avatar = $"{Path.GetRandomFileName()}.{Path.GetExtension(model.Avatar.FileName)}";
                    await model.Avatar.WriteToFile(Path.Combine(userDir, person.Avatar));
                }
                person.Login = model.Username;
                if (!string.IsNullOrEmpty(model.NewPassword))
                    person.PasswordHash = Argon2.Hash(model.NewPassword);
                person.Role = model.Role;
                await context.SaveChangesAsync();
                return Redirect("/Home/Index");
            }
            else
            {
                ModelState.AddModelError("", "Неверный ID");
                return (View(model));
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserDetails(int id)
        {
            Person person = context.People.Find(id);
            return View(person.ToUserModel());
        }


        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            Person person = context.People.Find(id);
            return View(person.ToUserModel());
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DelUser(int id)
        {
            Person person = context.People.Find(id);
            if (person != null)
            {
                context.People.Remove(person);
                await context.SaveChangesAsync();
            }
            return Redirect("ListUsers");
        }

        [Authorize]
        public IActionResult Userpage()
        {
            UserModel usr = CurrentUser.ToUserModel();
            usr.Addres = new List<AddressModel>();
            IQueryable<Address> allAdresses = context.Address.Where(a=>a.Owner.Id == usr.Id);
            foreach(Address adress in allAdresses){
                usr.Addres.Add(new AddressModel(){
                    Id = adress.Id,
                    Addr = adress.Addr,
                    Description = adress.Description,
                    Cost = adress.Cost,
                    Rooms = adress.Rooms
                });
            }            
            return View(usr);
        }

        [Authorize]
        public async Task<IActionResult> DeleteAddress(int IdAddr)
        {
            Address AddrDelete = context.Address.Find(IdAddr);
            if (AddrDelete.Owner.Id == CurrentUser.Id){
                context.Address.Remove(AddrDelete);
                await context.SaveChangesAsync();   
            }        
            return Redirect("/Account/UserPage");
        }

        private string GetContentType(string filename)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filename, out contentType);
            return contentType ?? "application/octet-stream";
        }

        [Authorize]
        public async Task<IActionResult> Avatar(string username)
        {
            Person person = context.People.FirstOrDefault(p => p.Login == username);

            string filePath;
            if (person == null || person.Avatar == null)
                filePath = Path.Combine(hostEnvironment.ContentRootPath, "DefaultImages", "no_ava.png");
            else
                filePath = Path.Combine(ImageFolder, person.Id.ToString(), person.Avatar);
            string contentType = GetContentType(filePath);
            byte[] imgBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(imgBytes, contentType);
        }
        [Authorize]
        [HttpGet]
        public IActionResult RegisterAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAddress(AddressModel model)
        {

            Address address = new Address
            {
                Addr = model.Addr,
                Description = model.Description,
                Owner = CurrentUser,
                Cost = model.Cost,
                Rooms = model.Rooms
            };
            await context.Address.AddAsync(address);
            await context.SaveChangesAsync();
            return Redirect("/Account/UserPage");
        }

    }
}
