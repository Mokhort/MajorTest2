using JwtTest.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtTest.Controllers
{
    public abstract class BaseController : Controller
    {

        protected JwtContext context;
        protected IOptions<AuthOptions> options;
        protected IHostEnvironment hostEnvironment;
        protected string ImageFolder
        {
            get
            {
                return Path.Combine(hostEnvironment.ContentRootPath, "Images");
            }
        }

        protected async Task Authenticate(string userName, UserRole role)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Enum.GetName(role))
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        protected async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private Person currentUser;

        protected Person CurrentUser
        {
            get
            {
                if (currentUser == null)
                    currentUser = context.People.SingleOrDefault(p => User.Identities.First().Name == p.Login);
                return currentUser;

            }
        }
    }
}
