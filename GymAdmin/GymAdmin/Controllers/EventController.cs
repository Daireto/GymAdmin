using Microsoft.AspNetCore.Mvc;
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
    public class EventController : Controller
    {
        //[Authorize(Roles = "Admin")]
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;

        public EventController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
        }
        //------------------------------------------------------------ EVENTS -------------------------------------------
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events
               .Include(d => d.Directors)
               .ThenInclude(p => p.User)
               .ToListAsync());
            //return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event envet = await _context.Events
                .Include(e => e.Directors)
                .ThenInclude(d => d.Schedule)
                .Include(e => e.Directors)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (envet == null)
            {
                return NotFound();
            }

            return View(envet);
        }
        public async Task<IActionResult> Create()
        {
            AddEventViewModel model = new()
            {
                Directors = await _combosHelper.GetComboProfessionalsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Director director = await _context.Directors.FirstOrDefaultAsync(p => p.User.Email == model.DirectorUserName);
                if (director == null)
                {
                    ModelState.AddModelError(string.Empty, "¡El Director no existe en el sistema, debe crearlo antes de crear el servicio!");
                    model.Directors = await _combosHelper.GetComboProfessionalsAsync();
                    return View(model);
                }

                Event evento = new()
                {
                    NameEveneto = model.Name,
                    Director = await _context.Directors.FirstOrDefaultAsync(p => p.User.UserName == model.DirectorUserName)
                };

                try
                {
                    _context.Add(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Evento con este nombre");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Directors = await _combosHelper.GetComboDirectorsAsync();
            return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event evento = await _context.Events
                .Include(d => d.Directors)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            AddEventViewModel model = new()
            {
                Id = id,
                Name = evento.NameEveneto,
                DirectorUserName = evento.Director.User.UserName,
                Directors = await _combosHelper.GetComboDirectorsAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddEventViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Event evento = await _context.Events.FindAsync(model.Id);
                    evento.NameEveneto= model.Name;                    
                    evento.Director= await _context.Directors.FirstOrDefaultAsync(p => p.User.UserName == model.DirectorUserName);

                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un evento con este nombre");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Directors = await _combosHelper.GetComboDirectorsAsync();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event evento = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Event evento = await _context.Events.FindAsync(id);
            _context.Events.Remove(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //--------------                    Director                -------------------

        public async Task<IActionResult> DetailsDirector(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Director director   = await _context.Directors
                .Include(p => p.User)
                .Include(p => p.Schedule)
                .Include(p => p.Events)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(director);
        }
        public async Task<IActionResult> CreateProfessional()
        {
            AddProfessionalViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = UserType.User,
                Schedules = await _combosHelper.GetComboSchedulesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirector(AddDirectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ScheduleId == 0)
                {
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un horario!");
                    return View(model);
                }

                User userDocumentExist = await _userHelper.GetUserAsync(model);
                if (userDocumentExist != null)
                {
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
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
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                    ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
                    return View(model);
                }

                //Relation user - professional
                Professional professional = new()
                {
                    User = user,
                    ProfessionalType = model.ProfessionalType,
                    Schedule = await _context.Schedules.FindAsync(model.ScheduleId)
                };
                _context.Add(professional);
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

            model.Schedules = await _combosHelper.GetComboSchedulesAsync();
            return View(model);
        }

    }
}
