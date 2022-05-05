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


    }
}
