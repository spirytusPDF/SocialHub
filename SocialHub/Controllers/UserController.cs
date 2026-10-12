using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialHub.Context;
using SocialHub.Models;
using System.Security.Claims;

namespace SocialHub.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }


        
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Login == username.Trim());

            if (user != null)
            {
                if (user.Password == password)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                   };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Posts", "SocialHub");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Id,Login,PhoneNumber,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                var userCheck = new User();
                try
                {
                    userCheck = await _context.Users.FindAsync(user.Login);
                }
                catch (Exception)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Username", "This login is already taken. Please choose another.");
                        return View(); 
                    }
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return View("Login");
                }
                return View("Login");

                
            }
            return NotFound();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Posts", "SocialHub");
        }
    }
}
