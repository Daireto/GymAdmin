using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
    public class DirectorController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;

        public DirectorController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DetailsDirector(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Director director = await _context.Directors
                .Include(d => d.User)
                .Include(d => d.Events)              
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(director);
        }
        public async Task<IActionResult> CreateDirector()
        {
            AddDirectorViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = UserType.User,
                Events = await _combosHelper.GetComboEventsAsync()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirector(AddDirectorViewModel model)
        {
            if (ModelState.IsValid)
            {              
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un horario para el evento a dar!");
                    return View(model);               

                User userDocumentExist = await _userHelper.GetUserAsync(model);
                if (userDocumentExist != null)
                {
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Ya existe un usuario con este documento!");
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
                    await _blobHelper.DeleteBlobAsync(imageId, "users");
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
                    return View(model);
                }

                //Relation user - professional
                Director director= new()
                {
                    User = user,
                    DirectorType = model.DirectorType,
                    //Event = await _context.Events.FindAsync(model.EventId)
                };
                _context.Add(director);
                await _context.SaveChangesAsync();

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

            model.Events = await _combosHelper.GetComboEventsAsync();
            return View(model);
        }
        public async Task<IActionResult> EditDirector(int? id)
        {
            EditDirectorViewModel model = new()
            {
                Users = await _combosHelper.GetComboUsersAsync(),
                Events = await _combosHelper.GetComboEventsAsync()
            };

            if (id != null)
            {
                Director director = await _context.Directors
                    .Include(d => d.Events)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (director != null)
                {
                    model.Id = director.Id;
                    model.Username = director.User.UserName;
                    model.EventId = director.Id;//lleva el id del director , porque lo crea este
                    model.DirectorType = director.DirectorType;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDirector(EditDirectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == null || model.Username == "")
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un usuario!");
                    return View(model);
                }

                if (model.ScheduleId == 0)
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un horario para el evento!");
                    return View(model);
                }

                User user = await _userHelper.GetUserAsync(model.Username);
                if (user == null)
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Events = await _combosHelper.GetComboEventsAsync();
                    ModelState.AddModelError(string.Empty, "¡Este usuario no existe en el sistema!");
                    return View(model);
                }

                Director director = new();

                if (model.Id != null)
                {
                    director = await _context.Directors.FindAsync(model.Id);
                    director.User = await _userHelper.GetUserAsync(model.Username);
                    director.DirectorType= model.DirectorType;
                    //director.Events = await _context.Events.FindAsync(model.EventId);
                    _context.Update(director);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        director = new()
                        {
                            User = await _userHelper.GetUserAsync(model.Username),
                            DirectorType = model.DirectorType,
                           // Schedule = await _context.Schedules.FindAsync(model.ScheduleId)
                        };
                        _context.Add(director);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "¡Este usuario ya tiene esta profesión!");
                        model.Users = await _combosHelper.GetComboUsersAsync();
                        model.Events = await _combosHelper.GetComboEventsAsync();
                        return View(model);
                    }
                }

                return RedirectToAction(nameof(DetailsDirector), new { id = director.Id });
            }

            model.Users = await _combosHelper.GetComboUsersAsync();
            model.Events = await _combosHelper.GetComboEventsAsync();
            return View(model);
        }

        public async Task<IActionResult> DeleteDirector(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Director director= await _context.Directors
                .Include(d => d.User)
                .Include(d => d.Events)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }
        [HttpPost, ActionName("DeleteProfessional")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDirectorConfirmed(int id)
        {
            Director director= await _context.Directors
                .Include(d => d.Events)
                .FirstOrDefaultAsync(m => m.Id == id);

            foreach (var evento in director.Events)
            {
                _context.Events.Remove(evento);
            }

            _context.Directors.Remove(director);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowDirector));
        }

        public async Task<IActionResult> ShowDirector()
        {
            return View(await _context.Directors
                .Include(d => d.Events)
                .Include(d => d.User)
                .ToListAsync());
        }
        //---------------------------- Events -----------------
    }
}
