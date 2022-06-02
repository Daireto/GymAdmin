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
    public class EventsController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public EventsController(DataContext context, IUserHelper userHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "NameDesc" : "";
            ViewData["DaySortParm"] = sortOrder == "Day" ? "DayDesc" : "Day";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null) { pageNumber = 1; }
            else { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;


            IQueryable<Event> query = _context.Events
                .Include(e => e.EventImages)
                .Include(e => e.Director)
                .ThenInclude(d => d.User);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => (
                e.Name.ToLower().Contains(searchString.ToLower())));
            }

            query = sortOrder switch
            {
                "NameDesc" => query.OrderByDescending(e => e.Name),
                "Day" => query.OrderBy(e => ((int)e.Day)),
                "DayDesc" => query.OrderByDescending(e => ((int)e.Day)),
                _ => query.OrderBy(e => e.Name)
            };

            int pageSize = 6;
            EventsViewModel model = new() { Events = await PaginatedList<Event>.CreateAsync(query, pageNumber ?? 1, pageSize) };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowEventInscriptions()
        {
            return View(await _context.EventInscriptions
                .Include(ei => ei.User)
                .Include(ei => ei.Event)
                .ThenInclude(e => e.Director)
                .ThenInclude(d => d.User)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsEventInscription(int id)
        {
            return View(await _context.EventInscriptions
                .Include(ei => ei.User)
                .Include(ei => ei.Event)
                .ThenInclude(e => e.Director)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(ei => ei.Id == id));
        }

        [NoDirectAccess]
        public async Task<IActionResult> SignUpToEvent(int id)
        {
            Event objectEvent = await _context.Events
                .Include(e => e.EventImages)
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            SignUpToEventViewModel model = new()
            {
                Day = objectEvent.Day,
                Description = objectEvent.Description,
                DirectorFullName = objectEvent.Director.User.FullName,
                EventId = objectEvent.Id,
                EventType = objectEvent.EventType,
                FinishHour = objectEvent.FinishHour,
                Name = objectEvent.Name,
                StartHour = objectEvent.StartHour,
                EventImages = objectEvent.EventImages,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpToEvent(SignUpToEventViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pI => pI.User)
                .Include(pI => pI.Plan)
                .FirstOrDefaultAsync(
                    pI => pI.User.UserName == User.Identity.Name &&
                    pI.PlanStatus == PlanStatus.Active
                );

                if (planInscription == null)
                {
                    if (User.IsInRole("User"))
                    {
                        _flashMessage.Danger("No tienes ningún plan activo, debes adquirir uno para inscribirte a un evento", "Error:");
                        return Json(new { isValid = true, redirect = true, route = "Account/ViewUser" });
                    }
                    else
                    {
                        return Json(new { isValid = true, redirect = true, route = "Account/ViewUser" });
                    }
                }

                Event objectEvent = await _context.Events.FindAsync(model.EventId);

                EventInscription ei = await _context.EventInscriptions
                    .FirstOrDefaultAsync(ei =>
                        ei.Event.Id == objectEvent.Id &&
                        ei.User.Email == User.Identity.Name &&
                        ei.EventStatus == EventStatus.SignedUp
                    );

                if (ei == null)
                {
                    User user = await _userHelper.GetUserAsync(User.Identity.Name);

                    ei = new()
                    {
                        Event = objectEvent,
                        EventStatus = EventStatus.SignedUp,
                        InscriptionDate = DateTime.Now,
                        User = user
                    };

                    _context.Add(ei);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Inscripción realizada correctamente", "Operación exitosa:");
                    return Json(new { isValid = true, redirect = true, route = "Home/MyEvents" });
                }
                else
                {
                    _flashMessage.Warning("Ya estás inscrito/a a este evento", "Error:");
                    return Json(new { isValid = true, redirect = true, route = "Events/Index" });
                }
            }
            return Json(new { isValid = true, redirect = true, route = "Account/Login" });
        }

        [NoDirectAccess]
        public async Task<IActionResult> CancelInscription(int id)
        {
            EventInscription eventInscription = await _context.EventInscriptions.FindAsync(id);

            if (eventInscription.EventStatus == EventStatus.SignedUp)
            {
                eventInscription.EventStatus = EventStatus.Cancelled;
                _context.Update(eventInscription);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Inscripción cancelada correctamente", "Operación exitosa:");
            }
            else
            {
                _flashMessage.Danger("Sólo se pueden cancelar inscripciones activas", "Error:");
            }

            if (User.IsInRole("User"))
            {
                return RedirectToAction("MyEvents", "Home");
            }

            return RedirectToAction(nameof(DetailsEventInscription), new { id = id });
        }
    }
}
