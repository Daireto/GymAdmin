using GymAdmin.Common;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
        }
        //View user method
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

        //Login get method
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //Login post method
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
                    ModelState.AddModelError(string.Empty, "¡Ha alcanzado el número máximo de intentos! Intente de nuevo en 5 minutos");
                }
                else if (result.IsNotAllowed)
                {
                    //TODO: Change for a view in the case the confirmation email was not sent, with a button to try to send again
                    ModelState.AddModelError(string.Empty, "¡Este email no está verificado! Siga los pasos enviados al correo");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Email o contraseña incorrectos!");
                }
            }
            return View(model);
        }

        //Logout get method
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Register get method
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

        //Register post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            //TODO: Evitar registrar usuario con documento que ya existe
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.AddUserAsync(model, imageId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
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
                    return RedirectToAction("ConfirmEmailMessage", "Account");
                }
                else
                {
                    return RedirectToAction("ConfirmEmailErrorMessage", "Account");
                }
            }
            return View(model);
        }

        //View with the email confirmation message about instructions to do
        public IActionResult ConfirmEmailMessage()
        {
            return View();
        }

        //View with the error sending email message
        public IActionResult ConfirmEmailErrorMessage()
        {
            return View(); //TODO: Make the view (En caso de error al enviar el email)
        }

        //Confirm email method
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

        //Edit user get method
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

        //Edit post get method
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
                    return RedirectToAction("ViewUser", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error! Intente de nuevo más tarde");
                }
            }
            return View(model);
        }

        //Change password get method
        public IActionResult ChangePassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        //Change password post method
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
                        return RedirectToAction(nameof(ViewUser));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "¡La contraseña actual es incorrecta!");
                    }
                }
            }
            return View(model);
        }

        //Recover password get method
        public IActionResult RecoverPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Recover password post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "¡Este usuario no está registrado en el sistema!");
                    return View(model);
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
                    return RedirectToAction("RecoverPasswordMessage", "Account");
                }
                else
                {
                    return RedirectToAction("RecoverPasswordErrorMessage", "Account");
                }

            }
            return View(model);
        }

        //View with the recovering password message about instructions to do
        public IActionResult RecoverPasswordMessage()
        {
            return View();
        }

        //View with the error recovering password message
        public IActionResult RecoverPasswordErrorMessage()
        {
            return View();
        }

        //Reset password get method
        public IActionResult ResetPassword(string token)
        {
            ResetPasswordViewModel model = new()
            {
                Token = token
            };

            return View(model);
        }

        //Reset password post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "¡Este usuario no está registrado en el sistema!");
                    return View(model);
                }

                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordMessage", "Account");
                }
                else
                {
                    return RedirectToAction("RecoverPasswordErrorMessage", "Account");
                }
            }

            return View(model);
        }

        //View with the reseting password message
        public IActionResult ResetPasswordMessage()
        {
            return View();
        }
    }
}
