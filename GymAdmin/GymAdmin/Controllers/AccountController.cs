using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;

namespace GymAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public AccountController(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper, IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public async Task<IActionResult> ViewUser()
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
                else if (result.IsLockedOut)
                {
                    _flashMessage.Danger("Ha alcanzado el número máximo de intentos, intente de nuevo en 5 minutos", "Error:");
                }
                else if (result.IsNotAllowed)
                {
                    _flashMessage.Danger("Este email no está verificado, siga los pasos enviados al correo", "Error:");
                }
                else
                {
                    _flashMessage.Danger("Email o contraseña incorrectos", "Error:");
                }
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
                UserType = UserType.User,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userDocumentExist = await _userHelper.GetUserAsync(model);
                if (userDocumentExist != null)
                {
                    _flashMessage.Danger("Ya existe un usuario con este documento, por favor ingrese otro", "Error:");
                    return View(model);
                }

                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.AddUserAsync(model, imageId);
                if (user == null)
                {
                    _flashMessage.Danger("Este correo ya está en uso, por favor ingrese otro", "Error:");
                    return View(model);
                }

                //Email confirmation
                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new
                    {
                        UserId = user.Id,
                        Token = token
                    },
                    protocol: HttpContext.Request.Scheme);

                string body = "<style>body{text-align:center;font-family:Verdana,Arial;}</style>" +
                    $"<h1>Soporte GymAdmin</h1>" +
                    $"<h3>Estás a un solo paso de ser parte de nuestra comunidad</h3>" +
                    $"<h4>Sólo debes hacer click en el siguiente botón para confirmar tu email</h4>" +
                    $"<br/>" +
                    $"<a style=\"padding:15px;background-color:#f1b00e;text-decoration:none;color:black;border: 5px solid #000;border-radius:10px;\" href=\"{tokenLink}\">Confirmar email</a>";

                Response response = _mailHelper.SendMail(
                    user.FullName,
                    model.Username,
                    "GymAdmin - Confirmación del email",
                    body);

                if (response.IsSuccess)
                {
                    _flashMessage.Confirmation("Sigue las instrucciones enviadas a tu correo", "Para continuar debes verificar tu email:");
                }
                else
                {
                    _flashMessage.Danger("Si el problema persiste comunicate con soporte técnico", "Ha ocurrido un error:");
                }
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(UserId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, Token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

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
                if (model.ImageFile != null)
                {
                    if (imageId != Guid.Empty)
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
                user.PhoneNumber = model.PhoneNumber;
                user.ImageId = imageId;
                var result = await _userHelper.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    _flashMessage.Confirmation("Perfil actualizado correctamente", "Operación exitosa:");
                    return RedirectToAction("ViewUser", "Account");
                }
                else
                {
                    _flashMessage.Danger("Si el problema persiste comunicate con soporte técnico", "Ha ocurrido un error:");
                }
            }
            return View(model);
        }

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
                    _flashMessage.Danger("La contraseña nueva debe ser diferente de la actual", "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "ChangePassword", model) });
                }
                else
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        _flashMessage.Confirmation("Contraseña actualizada correctamente", "Operación exitosa:");
                        return RedirectToAction(nameof(ViewUser));
                    }
                    else
                    {
                        _flashMessage.Danger("La contraseña actual es incorrecta", "Error:");
                    }
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "ChangePassword", model) });
        }

        public IActionResult RecoverPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    _flashMessage.Danger("Este usuario no está registrado en el sistema", "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "RecoverPassword", model) });
                }

                //Email confirmation
                string token = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string tokenLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        Token = token
                    },
                    protocol: HttpContext.Request.Scheme);

                string body = "<style>body{text-align:center;font-family:Verdana,Arial;}</style>" +
                    $"<h1>Soporte GymAdmin</h1>" +
                    $"<h3>Recuperación de contraseña</h3>" +
                    $"<h4>Para recuperar tu acceso a GymAdmin haz click en el siguiente botón</h4>" +
                    $"<br/>" +
                    $"<a style=\"padding:15px;background-color:#f1b00e;text-decoration:none;color:black;border: 5px solid #000;border-radius:10px;\" href=\"{tokenLink}\">Recuperar contraseña</a>";

                Response response = _mailHelper.SendMail(
                    user.FullName,
                    model.Email,
                    "GymAdmin - Recuperación de contraseña",
                    body);

                if (response.IsSuccess)
                {
                    _flashMessage.Confirmation("Las instrucciones han sido enviadas al correo", "Recuperación de contraseña:");
                }
                else
                {
                    _flashMessage.Danger("Si el problema persiste comunicate con soporte técnico", "Ha ocurrido un error:");
                }

                return RedirectToAction(nameof(Login));
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "RecoverPassword", model) });
        }

        public IActionResult ResetPassword(string token)
        {
            ResetPasswordViewModel model = new()
            {
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.UserName);
                if (user == null)
                {
                    _flashMessage.Danger("Este usuario no está registrado en el sistema", "Ha ocurrido un error:");
                    return View(model);
                }

                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _flashMessage.Confirmation("Contraseña recuperada correctamente", "Operación exitosa:");
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    _flashMessage.Danger("Por favor compruebe las credenciales e intente de nuevo", "Error:");
                }
            }

            return View(model);
        }
    }
}
