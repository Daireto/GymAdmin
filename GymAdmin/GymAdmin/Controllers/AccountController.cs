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

        //Login and logout methods
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
                ModelState.AddModelError(string.Empty, "¡Email o contraseña incorrectos!");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Register user methods
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
                    ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
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

        //Edit user methods
        public async Task<IActionResult> EditUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
                EditUserViewModel model = new()
                {
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Document = user.Document,
                    DocumentType = user.DocumentType,
                    ImageId = user.ImageId,
                };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;
                if(model.ImageFile != null)
                {
                    if(imageId != Guid.Empty)
                    {
                        await _blobHelper.DeleteBlobAsync(imageId, "users");
                    }
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Document = model.Document;
                user.DocumentType = model.DocumentType;
                user.ImageId = imageId;
                var result = await _userHelper.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error! Intente de nuevo más tarde");
                }
            }
            return View(model);
        }

        //Change password methods
        public IActionResult ChangePassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
                if (model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "¡La contraseña nueva debe ser diferente de la actual!");
                }
                else
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(EditUser));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "¡La contraseña actual es incorrecta!");
                    }
                }
            }
            return View(model);
        }
    }
}
