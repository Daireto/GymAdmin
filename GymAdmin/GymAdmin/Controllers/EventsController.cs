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
        public async Task<IActionResult> ShowEventInscriptions() //TODO: Make view
        {
            return View(await _context.EventInscriptions
                .Include(ei => ei.User)
                .Include(ei => ei.Event)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsEventInscription(int id) //TODO: Make view
        {
            return View(await _context.EventInscriptions
                .Include(ei => ei.User)
                .Include(ei => ei.Event)
                .ThenInclude(e => e.Director)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(ei => ei.Id == id));
        }

        [NoDirectAccess]
        public async Task<IActionResult> SignUpToEvent(int id) //TODO: Make modal
        {
            if (User.Identity.IsAuthenticated)
            {
                Event objectEvent = await _context.Events.FindAsync(id);

                EventInscription ei = await _context.EventInscriptions
                    .FirstAsync(ei =>
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
                    return RedirectToAction("MyEvents", "Home");
                }
                else
                {
                    _flashMessage.Danger("Ya estás inscrito a este evento", "Error:");
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [NoDirectAccess]
        public async Task<IActionResult> CancelInscription(int id, string? route) //TODO: Make modal
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

            if (route == null)
            {
                return RedirectToAction(nameof(DetailsEventInscription), new { id = id });
            }

            if (route == "MyEvents")
            {
                return RedirectToAction("MyEvents", "Home");
            }
            return RedirectToAction(nameof(DetailsEventInscription), new { id = id });
        }
    }
}
