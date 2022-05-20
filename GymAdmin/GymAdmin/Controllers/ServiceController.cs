using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GymAdmin.Helpers.ModalHelper;

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
        private readonly IFlashMessage _flashMessage;

        public ServiceController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper, IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        //------------------------------------- Services --------------------------------------------

        [NoDirectAccess]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Services
                .Include(s => s.Professionals)
                .ThenInclude(p => p.User)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> Details(int? id)
        {
            Service service = await _context.Services
                .Include(s => s.Professionals)
                .ThenInclude(s => s.User)
                .Include(s => s.Professionals)
                .ThenInclude(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(service);
        }

        [NoDirectAccess]
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
                    _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un servicio con este nombre, por favor ingrese otro", "Error:");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message, "Error:");
                    }
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllServices", _context.Services
                            .Include(s => s.Professionals)
                            .ThenInclude(p => p.User)
                            .ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            Service service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro actualizado correctamente", "Operación exitosa:");
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un servicio con este nombre, por favor ingrese otro", "Error:");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message, "Error:");
                    }
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllServices", _context.Services
                            .Include(s => s.Professionals)
                            .ThenInclude(p => p.User)
                            .ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Service service = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            try
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el servicio porque tiene registros relacionados", "Error:");
            }
            return RedirectToAction(nameof(Index));
        }

        //------------------------------------- Professionals --------------------------------------------
        [NoDirectAccess]
        public async Task<IActionResult> ShowProfessionals()
        {
            return View(await _context.Professionals
                .Include(p => p.User)
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> DetailsProfessional(int? id)
        {
            Professional professional = await _context.Professionals
                .Include(p => p.User)
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(professional);
        }

        [NoDirectAccess]
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
                    _flashMessage.Confirmation("Las instrucciones han sido enviadas al correo", "Para continuar se debe verificar el email:");
                }
                else
                {
                    _flashMessage.Danger("Si el problema persiste comunicate con soporte técnico", "Ha ocurrido un error:");
                }
                return RedirectToAction(nameof(ShowProfessionals));
            }
            return View(model);
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditProfessional(int? id)
        {
            EditProfessionalViewModel model = new()
            {
                Users = await _combosHelper.GetComboUsersAsync(),
            };

            Professional professional = await _context.Professionals
                .Include(p => p.Service)
                .Include(p => p.ProfessionalSchedules)
                .ThenInclude(ps => ps.Schedule)
                .FirstOrDefaultAsync(p => p.Id == id);

            model.Id = professional.Id;
            model.Username = professional.User.UserName;
            model.ProfessionalType = professional.ProfessionalType;

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
                    _flashMessage.Danger("Debe seleccionar un usuario", "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditProfessional", model) });
                }

                User user = await _userHelper.GetUserAsync(model.Username);
                if (user == null)
                {
                    _flashMessage.Danger("Este usuario no existe en el sistema", "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditProfessional", model) });
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
                    _flashMessage.Confirmation("Registro actualizado correctamente", "Operación exitosa:");
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
                        _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                    }
                    catch (Exception)
                    {
                        _flashMessage.Danger("Este usuario ya tiene esta profesión", "Error:");
                        return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditProfessional", model) });
                    }
                }

                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProfessionals", _context.Professionals
                        .Include(p => p.User)
                        .Include(p => p.ProfessionalSchedules)
                        .ThenInclude(ps => ps.Schedule)
                        .ToList())
                });
            }
            model.Users = await _combosHelper.GetComboUsersAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditProfessional", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteProfessional(int? id)
        {
            Professional professional = await _context.Professionals
                .Include(p => p.ProfessionalSchedules)
                .FirstOrDefaultAsync(m => m.Id == id);

            try
            {
                foreach (ProfessionalSchedule ps in professional.ProfessionalSchedules)
                {
                    _context.ProfessionalSchedules.Remove(ps);
                }
                _context.Professionals.Remove(professional);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar al profesional porque tiene registros relacionados", "Error:");
            }

            return RedirectToAction(nameof(ShowProfessionals));
        }

        //------------------------------------- Schedules --------------------------------------------
        [NoDirectAccess]
        public IActionResult CreateSchedule(int? id)
        {
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
                    _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateSchedule", model) });
                }

                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllSchedules", _context.Professionals
                            .Include(p => p.User)
                            .Include(p => p.ProfessionalSchedules)
                            .ThenInclude(ps => ps.Schedule)
                            .FirstOrDefault(p => p.Id == model.ProfessionalId))
                });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateSchedule", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditSchedule(int? id, int? proId)
        {
            Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);

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
            if (ModelState.IsValid)
            {
                Schedule schedule = await _context.Schedules.FindAsync(id);

                Professional professional = await _context.Professionals.FindAsync(model.ProfessionalId);

                ProfessionalSchedule ps = await _context.ProfessionalSchedules.FirstOrDefaultAsync(
                            ps => ps.Professional == professional && ps.Schedule == schedule);

                Schedule schedule2 = await _context.Schedules.FirstOrDefaultAsync(
                            s => s.Day == model.Day && s.StartHour == model.StartHour && s.FinishHour == model.FinishHour);

                if (schedule2 == null)
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
                _flashMessage.Confirmation("Registro actualizado correctamente", "Operación exitosa:");

                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllSchedules", _context.Professionals
                            .Include(p => p.User)
                            .Include(p => p.ProfessionalSchedules)
                            .ThenInclude(ps => ps.Schedule)
                            .FirstOrDefault(p => p.Id == model.ProfessionalId))
                });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditSchedule", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteSchedule(int? id, int? proId)
        {
            Schedule schedule = await _context.Schedules
                .FirstOrDefaultAsync(m => m.Id == id);

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
            Schedule schedule = await _context.Schedules.FindAsync(id);

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

            _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            return RedirectToAction(nameof(DetailsProfessional), new { id = model.ProfessionalId });
        }
    }
}
