using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = Enums.UserType.User
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if(model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userHelper.AddUserAsync(model, imageId);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está en uso");
                    return View(model);
                }
                LoginViewModel loginModel = new()
                {
                    Username = model.Username,
                    Password = model.Password,
                    RememberMe = false
                };
                var result = await _userHelper.LoginAsync(loginModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}
