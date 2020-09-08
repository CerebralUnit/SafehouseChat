using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Safehouse.Chat.Models;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;

namespace Safehouse.Chat.Controllers
{
    public class Users : Controller
    {
        IUserService users; 
        IChatGroupService chatGroups;
        public Users(IUserService userService,
            IChatGroupService chatGroupService)
        {
            users = userService;
            chatGroups = chatGroupService;

        }
         
        public async Task<IActionResult> Index(string id)
        {
            User user = await users.GetUser(id);

            if (user == null)
                return Redirect("login");
            
            return View("Profile", user);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await users.GetUserByUsernameOrEmail(model.Username);

                if(user != null)
                {  
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        await CreateCookie(user.Id, user.Email, true);
                        return RedirectToAction("Index", new { id = user.Id });
                    }

                }    
            }

            return View();
        }

        private async Task CreateCookie(string id, string email, bool persist)
        { 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id),
                new Claim(ClaimTypes.Email, email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principle = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties()
            {
                IsPersistent = persist,
                ExpiresUtc = DateTime.UtcNow.AddYears(2)
            };

            await HttpContext.SignInAsync(principle, authenticationProperties); 
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginModel model)
        {
            if (ModelState.IsValid)
            { 
                var hashedPw = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var createdUserId = await users.Create(new User()
                {
                    Email = model.Email,
                    Username = model.Username,
                    Password = hashedPw
                });

                if (!String.IsNullOrWhiteSpace(createdUserId))
                {
                    var user = users.GetUser(model.Username);
                    return Redirect($"{user.Id}");
                }
            }
            return View();
        }
    }
}
