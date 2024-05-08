using CopyNPaste.Core.Data;
using CopyNPaste.Core.Entities.Identity;
using CopyNPaste.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CopyNPaste.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationUserManager userManager
            , SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home", null);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", null);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                };
                var newUser = await _userManager.CreateAsync(user, model.Password);
                 await _signInManager.SignInAsync(user, true);

                return RedirectToAction("Index", "Home", null);
            }
            return View(model);
        }

        [Route("personalArea")]
        public async Task<IActionResult> PersonalArea()
        {
            var userName = User.Identity.Name;
            var user = _context.Users.Include(x => x.Copies).Where(x => x.UserName == userName).FirstOrDefault();
            if (user == null)
            {
                RedirectToAction("Index", "Home", null);
            }
            var viewModel = new PersonalAreaViewModel
            {
                Email = user.Email,
                Copies = user.Copies.ToList()
            };

            return View(viewModel);
        }
    }
}
