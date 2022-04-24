using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;

        public ServiceController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
        }

        //------------------------------------- Services --------------------------------------------

        public async Task<IActionResult> Index()
        {
            return View(await _context.Services
                .Include(s => s.Professional)
                .ThenInclude(p => p.User)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Services
                .Include(s => s.Professional)
                .ThenInclude(p => p.Schedule)
                .Include(s => s.Professional)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        public async Task<IActionResult> Create()
        {
            AddServiceViewModel model = new()
            {
                Professionals = await _combosHelper.GetComboProfessionalsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                Professional professional = await _context.Professionals.FirstOrDefaultAsync(p => p.User.Email == model.ProfessionalUserName);
                if(professional == null)
                {
                    ModelState.AddModelError(string.Empty, "¡El profesional no existe en el sistema, debe crearlo antes de crear el servicio!");
                    model.Professionals = await _combosHelper.GetComboProfessionalsAsync();
                    return View(model);
                }

                Service service = new()
                {
                    Name = model.Name,
                    Price = model.Price,
                    Professional = await _context.Professionals.FirstOrDefaultAsync(p => p.User.UserName == model.ProfessionalUserName)
                };

                try
                {
                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un servicio con este nombre");
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

            model.Professionals = await _combosHelper.GetComboProfessionalsAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Services
                .Include(s => s.Professional)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            AddServiceViewModel model = new()
            {
                Id = id,
                Name = service.Name,
                Price = service.Price,
                ProfessionalUserName = service.Professional.User.UserName,
                Professionals = await _combosHelper.GetComboProfessionalsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddServiceViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Service service = await _context.Services.FindAsync(model.Id);
                    service.Name = model.Name;
                    service.Price = model.Price;
                    service.Professional = await _context.Professionals.FirstOrDefaultAsync(p => p.User.UserName == model.ProfessionalUserName);

                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un servicio con este nombre");
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

            model.Professionals = await _combosHelper.GetComboProfessionalsAsync();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Service service = await _context.Services.FindAsync(id);
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //------------------------------------- Professionals --------------------------------------------

        public async Task<IActionResult> DetailsProfessional(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Professional professional = await _context.Professionals
                .Include(p => p.User)
                .Include(p => p.Schedule)
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(professional);
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
        public async Task<IActionResult> CreateProfessional(AddProfessionalViewModel model)
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

        public async Task<IActionResult> EditProfessional(int? id)
        {
            EditProfessionalViewModel model = new()
            {
                Users = await _combosHelper.GetComboUsersAsync(),
                Schedules = await _combosHelper.GetComboSchedulesAsync()
            };

            if (id != null)
            {
                Professional professional = await _context.Professionals
                    .Include(p => p.Schedule)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (professional != null)
                {
                    model.Id = professional.Id;
                    model.Username = professional.User.UserName;
                    model.ScheduleId = professional.Schedule.Id;
                    model.ProfessionalType = professional.ProfessionalType;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfessional(EditProfessionalViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == null || model.Username == "")
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un usuario!");
                    return View(model);
                }

                if (model.ScheduleId == 0)
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un horario!");
                    return View(model);
                }

                User user = await _userHelper.GetUserAsync(model.Username);
                if(user == null)
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                    ModelState.AddModelError(string.Empty, "¡Este usuario no existe en el sistema!");
                    return View(model);
                }

                Professional professional = new();

                if (model.Id != null)
                {
                    professional = await _context.Professionals.FindAsync(model.Id);
                    professional.User = await _userHelper.GetUserAsync(model.Username);
                    professional.ProfessionalType = model.ProfessionalType;
                    professional.Schedule = await _context.Schedules.FindAsync(model.ScheduleId);
                    _context.Update(professional);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        professional = new()
                        {
                            User = await _userHelper.GetUserAsync(model.Username),
                            ProfessionalType = model.ProfessionalType,
                            Schedule = await _context.Schedules.FindAsync(model.ScheduleId)
                        };
                        _context.Add(professional);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception exception)
                    {
                        ModelState.AddModelError(string.Empty, "¡Este usuario ya tiene esta profesión!");
                        model.Users = await _combosHelper.GetComboUsersAsync();
                        model.Schedules = await _combosHelper.GetComboSchedulesAsync();
                        return View(model);
                    }
                }

                return RedirectToAction(nameof(DetailsProfessional), new { id = professional.Id });
            }

            model.Users = await _combosHelper.GetComboUsersAsync();
            model.Schedules = await _combosHelper.GetComboSchedulesAsync();
            return View(model);
        }

        public async Task<IActionResult> DeleteProfessional(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Professional professional = await _context.Professionals
                .Include(p => p.User)
                .Include(p => p.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (professional == null)
            {
                return NotFound();
            }

            return View(professional);
        }

        [HttpPost, ActionName("DeleteProfessional")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfessionalConfirmed(int id)
        {
            Professional professional = await _context.Professionals
                .Include(p => p.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            foreach(var service in professional.Services)
            {
                _context.Services.Remove(service);
            }

            _context.Professionals.Remove(professional);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowProfessionals));
        }

        public async Task<IActionResult> ShowProfessionals()
        {
            return View(await _context.Professionals
                .Include(p => p.Schedule)
                .Include(p => p.User)
                .ToListAsync());
        }

        //------------------------------------- Schedules --------------------------------------------

        public async Task<IActionResult> ShowSchedules()
        {
            return View(await _context.Schedules.ToListAsync());
        }

        /*
         * SCHEDULE ACTIONS
         */

        //DetailsSchedule

        //CreateSchedule

        //EditSchedule

        //DeleteSchedule
    }
}
