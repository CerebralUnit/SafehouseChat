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
         
        public async Task<IActionResult> Index()
        { 
            User user = await users.GetUser(User.Identity.Name);
            List<User> friends = await users.GetFriends(User.Identity.Name);
            List<FriendRequest> pending = await users.GetPendingFriendRequests(User.Identity.Name); 

            ViewBag.Friends = friends;
            ViewBag.OnlineFriends = friends.Where(x => x.Online).ToList();
            ViewBag.PendingFriendRequests = pending;
            ViewBag.User = user;
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
                return await AuthenticateUser(model); 

            return View();
        }


        private async Task<IActionResult> AuthenticateUser(LoginModel model)
        {
            var user = await users.GetUserByUsernameOrEmail(model.Username);

            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    await CreateCookie(user.Id, user.Email, true);
                    return Redirect("/users/@me");
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
                    return await AuthenticateUser(model);
            }
            return View();
        }
    }
}
