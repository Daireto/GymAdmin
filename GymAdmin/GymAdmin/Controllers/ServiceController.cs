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
                .Include(s => s.Professionals)
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
                .Include(s => s.Professionals)
                .ThenInclude(s => s.User)
                .Include(s => s.Professionals)
                .ThenInclude(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service model)
        {
            if (ModelState.IsValid)
            {
                Service service = new()
                {
                    Name = model.Name,
                    Price = model.Price,
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

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
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
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(professional);
        }

        public IActionResult CreateProfessional()
        {
            AddProfessionalViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = UserType.User,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfessional(AddProfessionalViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userDocumentExist = await _userHelper.GetUserAsync(model);
                if (userDocumentExist != null)
                {
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
                    ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
                    return View(model);
                }

                //Relation user - professional
                Professional professional = new()
                {
                    User = user,
                    ProfessionalType = model.ProfessionalType,
                };

                switch (model.ProfessionalType)
                {
                    case ProfessionalType.Instructor:
                        professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Instrucción");
                        break;
                    case ProfessionalType.Physiotherapist:
                        professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Fisioterapia");
                        break;
                    case ProfessionalType.Nutritionist:
                        professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Nutricionismo");
                        break;
                    default:
                        break;
                }

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

            return View(model);
        }

        public async Task<IActionResult> EditProfessional(int? id)
        {
            EditProfessionalViewModel model = new()
            {
                Users = await _combosHelper.GetComboUsersAsync(),
            };

            if (id != null)
            {
                Professional professional = await _context.Professionals
                    .Include(p => p.Service)
                    .Include(p => p.ProfessionalSchedules)
                    .ThenInclude(ps => ps.Schedule)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (professional != null)
                {
                    model.Id = professional.Id;
                    model.Username = professional.User.UserName;
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
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un usuario!");
                    return View(model);
                }

                User user = await _userHelper.GetUserAsync(model.Username);
                if (user == null)
                {
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    ModelState.AddModelError(string.Empty, "¡Este usuario no existe en el sistema!");
                    return View(model);
                }

                Professional professional = new();

                if (model.Id != null)
                {
                    professional = await _context.Professionals.FindAsync(model.Id);
                    professional.User = await _userHelper.GetUserAsync(model.Username);
                    professional.ProfessionalType = model.ProfessionalType;

                    switch (model.ProfessionalType)
                    {
                        case ProfessionalType.Instructor:
                            professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Instrucción");
                            break;
                        case ProfessionalType.Physiotherapist:
                            professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Fisioterapia");
                            break;
                        case ProfessionalType.Nutritionist:
                            professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Nutricionismo");
                            break;
                        default:
                            break;
                    }

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
                        };

                        switch (model.ProfessionalType)
                        {
                            case ProfessionalType.Instructor:
                                professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Instrucción");
                                break;
                            case ProfessionalType.Physiotherapist:
                                professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Fisioterapia");
                                break;
                            case ProfessionalType.Nutritionist:
                                professional.Service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "Nutricionismo");
                                break;
                            default:
                                break;
                        }

                        _context.Add(professional);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "¡Este usuario ya tiene esta profesión!");
                        model.Users = await _combosHelper.GetComboUsersAsync();
                        return View(model);
                    }
                }

                return RedirectToAction(nameof(DetailsProfessional), new { id = professional.Id });
            }

            model.Users = await _combosHelper.GetComboUsersAsync();
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
                .Include(p => p.Service)
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
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
                .Include(p => p.ProfessionalSchedules)
                .FirstOrDefaultAsync(m => m.Id == id);

            foreach (ProfessionalSchedule ps in professional.ProfessionalSchedules)
            {
                _context.ProfessionalSchedules.Remove(ps);
            }

            _context.Professionals.Remove(professional);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowProfessionals));
        }

        public async Task<IActionResult> ShowProfessionals()
        {
            return View(await _context.Professionals
                .Include(p => p.User)
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .ToListAsync());
        }

        //------------------------------------- Schedules --------------------------------------------
        public IActionResult CreateSchedule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SetScheduleViewModel model = new()
            {
                ProfessionalId = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(SetScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(
                            s => s.Day == model.Day && s.StartHour == model.StartHour && s.FinishHour == model.FinishHour);

                if (schedule == null)
                {
                    schedule = new()
                    {
                        Day = model.Day,
                        StartHour = model.StartHour,
                        FinishHour = model.FinishHour,
                    };
                }

                ProfessionalSchedule ps = new()
                {
                    Professional = await _context.Professionals.FindAsync(model.ProfessionalId),
                    Schedule = schedule,
                };

                try
                {
                    _context.Add(ps);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsProfessional), new { id = model.ProfessionalId });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> EditSchedule(int? id, int? proId)
        {
            if (id == null || proId == null)
            {
                return NotFound();
            }

            Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            SetScheduleViewModel model = new()
            {
                Id = schedule.Id,
                Day = schedule.Day,
                StartHour = schedule.StartHour,
                FinishHour = schedule.FinishHour,
                ProfessionalId = proId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(int id, SetScheduleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Schedule schedule = await _context.Schedules.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound();
                }

                Professional professional = await _context.Professionals.FindAsync(model.ProfessionalId);

                ProfessionalSchedule ps = await _context.ProfessionalSchedules.FirstOrDefaultAsync(
                            ps => ps.Professional == professional && ps.Schedule == schedule);

                Schedule schedule2 = await _context.Schedules.FirstOrDefaultAsync(
                            s => s.Day == model.Day && s.StartHour == model.StartHour && s.FinishHour == model.FinishHour);
                if (schedule == null)
                {
                    schedule2 = new()
                    {
                        Day = model.Day,
                        StartHour = model.StartHour,
                        FinishHour = model.FinishHour,
                    };
                }

                ps.Schedule = schedule2;
                _context.Update(ps);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsProfessional), new { id = model.ProfessionalId });
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteSchedule(int? id, int? proId)
        {
            if (id == null || proId == null)
            {
                return NotFound();
            }

            Schedule schedule = await _context.Schedules
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            SetScheduleViewModel model = new()
            {
                Id = schedule.Id,
                Day = schedule.Day,
                StartHour = schedule.StartHour,
                FinishHour = schedule.FinishHour,
                ProfessionalId = proId,
            };

            return View(model);
        }

        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScheduleConfirmed(int id, SetScheduleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            Schedule schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            Professional professional = await _context.Professionals.FindAsync(model.ProfessionalId);

            ProfessionalSchedule ps = await _context.ProfessionalSchedules.FirstOrDefaultAsync(
                            ps => ps.Professional == professional && ps.Schedule == schedule);

            _context.ProfessionalSchedules.Remove(ps);
            await _context.SaveChangesAsync();

            if (await _context.ProfessionalSchedules.FirstOrDefaultAsync(ps => ps.Schedule == schedule) == null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(DetailsProfessional), new { id = model.ProfessionalId });
        }
    }
}
