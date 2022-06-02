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
    public class DirectorController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public DirectorController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper, IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        //------------------------------------- Directors --------------------------------------------
        public async Task<IActionResult> Index()
        {
            return View(await _context.Directors
                .Include(p => p.User)
                .Include(s => s.Events)
                .ThenInclude(e => e.EventImages)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> DetailsDirector(int id)
        {
            Director director = await _context.Directors
                .Include(d => d.User)
                .Include(d => d.Events)
                .ThenInclude(e => e.EventInscriptions)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(director);
        }

        public IActionResult CreateDirector()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                UserType = UserType.User
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirector(AddUserViewModel model)
        {
            if (await _context.Directors.Include(p => p.User).FirstOrDefaultAsync(p => p.User.UserName == model.Username) != null)
            {
                _flashMessage.Danger("Este usuario ya es un director", "Error:");
                return View(model);
            }

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

                Director director = new()
                {
                    User = user,
                };

                _context.Add(director);
                await _context.SaveChangesAsync();

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new
                    {
                        UserId = user.Id,
                        Token = token,
                        Route = "director"
                    },
                    protocol: HttpContext.Request.Scheme);

                string body = "<style>body{text-align:center;font-family:Verdana,Arial;}</style>" +
                    $"<h1>Soporte GymAdmin</h1>" +
                    $"<h3>Estás a un solo paso de ser director de eventos de GymAdmin</h3>" +
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
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditDirector(int? id)
        {
            EditDirectorViewModel model = new()
            {
                Users = await _combosHelper.GetComboNoDirectorsAsync(),
            };

            if (id != null)
            {
                Director director = await _context.Directors
                    .Include(d => d.User)
                    .Include(d => d.Events)
                    .FirstOrDefaultAsync(p => p.Id == id);

                model.Id = director.Id;
                model.Username = director.User.UserName;
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
                    _flashMessage.Danger("Debe seleccionar un usuario", "Error:");
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditDirector", model) });
                }

                User user = await _userHelper.GetUserAsync(model.Username);
                if (user == null)
                {
                    _flashMessage.Danger("Este usuario no existe en el sistema", "Error:");
                    model.Users = await _combosHelper.GetComboUsersAsync();
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditDirector", model) });
                }

                Director director = new();

                if (model.Id != null)
                {
                    director = await _context.Directors.FindAsync(model.Id);
                    director.User = await _userHelper.GetUserAsync(model.Username);

                    _context.Update(director);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro actualizado correctamente", "Operación exitosa:");
                }
                else
                {
                    try
                    {
                        director = new()
                        {
                            User = await _userHelper.GetUserAsync(model.Username),
                        };

                        _context.Add(director);
                        await _context.SaveChangesAsync();
                        _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                    }
                    catch
                    {
                        _flashMessage.Danger("Este usuario ya es un director", "Error:");
                        model.Users = await _combosHelper.GetComboUsersAsync();
                        return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditDirector", model) });
                    }
                }

                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllDirectors", _context.Directors
                        .Include(p => p.User)
                        .Include(s => s.Events)
                        .ToList())
                });
            }

            model.Users = await _combosHelper.GetComboUsersAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditDirector", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            Director director = await _context.Directors.FirstOrDefaultAsync(m => m.Id == id);

            try
            {
                _context.Directors.Remove(director);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar al director porque tiene registros relacionados", "Error:");
            }

            return RedirectToAction(nameof(Index));
        }

        //------------------------------------- Events --------------------------------------------
        public async Task<IActionResult> DetailsEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event eventObject = await _context.Events
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .Include(e => e.EventInscriptions.Where(ei => ei.EventStatus == EventStatus.SignedUp))
                .ThenInclude(ei => ei.User)
                .ThenInclude(u => u.PlanInscriptions)
                .ThenInclude(pi => pi.Plan)
                .Include(e => e.EventImages)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventObject == null)
            {
                return NotFound();
            }

            return View(eventObject);
        }

        [NoDirectAccess]
        public async Task<IActionResult> CreateEvent(int id)
        {
            Director director = await _context.Directors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            AddEventViewModel model = new()
            {
                DirectorUsername = director.User.Email,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(AddEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                try
                {
                    Director director = await _context.Directors
                        .Include(d => d.User)
                        .FirstOrDefaultAsync(d => d.User.Email == model.DirectorUsername);

                    Event objectEvent = new()
                    {
                        Day = model.Day,
                        StartHour = model.StartHour,
                        FinishHour = model.FinishHour,
                        Name = model.Name,
                        Director = director,
                        EventType = model.EventType,
                        Description = model.Description,
                        EventImages = new List<EventImage>()
                    };

                    if (model.ImageFile != null)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "events");

                        EventImage eventImage = new()
                        {
                            Event = objectEvent,
                            ImageId = imageId,
                        };

                        objectEvent.EventImages.Add(eventImage);
                    }

                    _context.Add(objectEvent);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un evento con este nombre, por favor ingrese otro", "Error:");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message, "Error:");
                    }
                    await _blobHelper.DeleteBlobAsync(imageId, "events");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    await _blobHelper.DeleteBlobAsync(imageId, "events");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllEvents", _context.Events
                            .Include(e => e.Director)
                            .ThenInclude(d => d.User)
                            .ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditEvent(int id)
        {
            Event eventObject = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            EditEventViewModel model = new()
            {
                Day = eventObject.Day,
                StartHour = eventObject.StartHour,
                FinishHour = eventObject.FinishHour,
                Name = eventObject.Name,
                EventType = eventObject.EventType,
                Description = eventObject.Description,
                Id = eventObject.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(EditEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Event objectEvent = await _context.Events.FindAsync(model.Id);
                    objectEvent.Day = model.Day;
                    objectEvent.StartHour = model.StartHour;
                    objectEvent.FinishHour = model.FinishHour;
                    objectEvent.Name = model.Name;
                    objectEvent.EventType = model.EventType;
                    objectEvent.Description = model.Description;

                    _context.Update(objectEvent);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro actualizado correctamente", "Operación exitosa:");
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un evento con este nombre, por favor ingrese otro", "Error:");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message, "Error:");
                    }
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditEvent", model) });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditEvent", model) });
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllEvents", _context.Events
                            .Include(e => e.Director)
                            .ThenInclude(d => d.User)
                            .ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditEvent", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            Event eventObject = await _context.Events
                .Include(e => e.Director)
                .Include(e => e.EventImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            try
            {
                if (eventObject.EventImages != null)
                {
                    foreach (EventImage eventImage in eventObject.EventImages)
                    {
                        try
                        {
                            await _blobHelper.DeleteBlobAsync(eventImage.ImageId, "events");
                            _context.EventImages.Remove(eventImage);
                        }
                        catch { }
                    }
                }

                _context.Events.Remove(eventObject);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el evento porque tiene registros relacionados", "Error:");
            }
            return RedirectToAction(nameof(DetailsDirector), new { id = eventObject.Director.Id });
        }

        [NoDirectAccess]
        public IActionResult AddEventImage(int id)
        {
            AddImageViewModel model = new()
            {
                EventId = id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEventImage(AddImageViewModel model)
        {
            try
            {
                if (model.ImageFile != null)
                {
                    Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "events");

                    Event objectEvent = await _context.Events
                        .Include(e => e.EventImages)
                        .FirstOrDefaultAsync(e => e.Id == model.EventId);

                    EventImage eventImage = new()
                    {
                        Event = objectEvent,
                        ImageId = imageId
                    };

                    if (objectEvent.EventImages == null)
                    {
                        objectEvent.EventImages = new List<EventImage>();
                    }
                    objectEvent.EventImages.Add(eventImage);

                    _context.Add(eventImage);
                    _context.Update(objectEvent);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Imagen insertada correctamente", "Operación exitosa:");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllImages", _context.Events
                            .Include(e => e.Director)
                            .ThenInclude(d => d.User)
                            .Include(e => e.EventInscriptions.Where(ei => ei.EventStatus == EventStatus.SignedUp))
                            .ThenInclude(ei => ei.User)
                            .ThenInclude(u => u.PlanInscriptions)
                            .ThenInclude(pi => pi.Plan)
                            .Include(e => e.EventImages)
                            .FirstOrDefault(e => e.Id == model.EventId))
                    });
                }
                else
                {
                    _flashMessage.Danger("Debe seleccionar una imagen", "Error:");
                }
            }
            catch
            {
                _flashMessage.Danger("La imagen no ha sido añadida", "Error:");
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddEventImage", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteEventImage(int id)
        {
            EventImage eventImage = await _context.EventImages
                .Include(ei => ei.Event)
                .FirstOrDefaultAsync(ei => ei.Id == id);

            Event objectEvent = await _context.Events
                .Include(e => e.EventImages)
                .FirstOrDefaultAsync(e => e.Id == eventImage.Event.Id);

            if(objectEvent.EventImages.Count == 1)
            {
                _flashMessage.Danger("El evento debe tener al menos una imagen", "Error:");
                return RedirectToAction(nameof(DetailsEvent), new { id = eventImage.Event.Id });
            }

            try
            {
                try
                {
                    await _blobHelper.DeleteBlobAsync(eventImage.ImageId, "events");
                }
                catch { }

                _context.Remove(eventImage);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Imagen eliminada correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("La imagen no pudo ser eliminada", "Error:");
            }
            return RedirectToAction(nameof(DetailsEvent), new { id = eventImage.Event.Id });
        }
    }
}
