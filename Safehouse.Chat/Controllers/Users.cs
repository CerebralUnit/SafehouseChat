using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Chat.Models;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;

namespace Safehouse.Chat.Controllers
{
    public class Users : Controller
    {
        IUserRepository users;
        IChannelRepository channels;
        public Users(IUserRepository userRepository,
            IChannelRepository channelRepository)
        {
            users = userRepository;
            channels = channelRepository;

        }
        public async Task<IActionResult> Index(string id)
        {
            User user = await users.RetrieveById(id);
            List<Core.Channel> userChannels = await channels.RetrieveMany(user.Channels);
            ViewBag.User = user;
            ViewBag.Channels = userChannels;
            return View("Profile");
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
                var user = await users.Retrieve(model.Username);

                if(user != null)
                {
                    var saltAndPw = user.Password.Split('.');
                    var hashedPw = BCrypt.Net.BCrypt.HashPassword(model.Password, saltAndPw[0]+".");

                    if (hashedPw == user.Password)
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
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var hashedPw = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

                var createdUserId = await users.Create(new User()
                {
                    Username = model.Username,
                    Password = hashedPw
                });

                if (!String.IsNullOrWhiteSpace(createdUserId))
                {
                    var user = users.Retrieve(model.Username);
                    return Redirect($"users/{user.Id}");
                }
            }
            return View();
        }
    }
}
