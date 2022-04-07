using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using GymAdmin.Models;
using GymAdmin.Enums;

namespace GymAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;

        public UserController(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .ToListAsync());
        }

        public IActionResult Create()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = UserType.Admin
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
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
    }
}
