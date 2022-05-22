﻿using GymAdmin.Common;
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
        }//TODO: Make views and the Seeder method for Directors, Events and Event inscriptions

        //------------------------------------- Directors --------------------------------------------
        public async Task<IActionResult> Index()
        {
            return View(await _context.Directors
                .Include(p => p.User)
                .Include(s => s.Events)
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
            if (await _context.Directors.Include(p => p.User).FirstAsync(p => p.User.UserName == model.Username) != null)
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
                Users = await _combosHelper.GetComboUsersAsync(),
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
                });//TODO: Make view _ViewAllDirectors
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
        }//TODO: Edit the deleteDialog script for Directors

        //------------------------------------- Events --------------------------------------------
        public async Task<IActionResult> ShowEvents()
        {
            return View(await _context.Events
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .ToListAsync());
        }

        public async Task<IActionResult> DetailsEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event eventObject = await _context.Events
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .Include(e => e.EventInscriptions)
                .ThenInclude(ei => ei.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventObject == null)
            {
                return NotFound();
            }

            return View(eventObject);
        }

        [NoDirectAccess]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(model);
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
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message, "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllEvents", _context.Events
                            .Include(e => e.Director)
                            .ThenInclude(d => d.User)
                            .ToList())
                });//TODO: Make view _ViewAllEvents
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateEvent", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditEvent(int id)
        {
            Event eventObject = await _context.Events
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(e => e.Id == id);
            return View(eventObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(Event model)
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
            Event eventObject = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            try
            {
                _context.Events.Remove(eventObject);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro eliminado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el evento porque tiene registros relacionados", "Error:");
            }
            return RedirectToAction(nameof(ShowEvents));
        }//TODO: Edit the deleteDialog script for Events
    }
}

