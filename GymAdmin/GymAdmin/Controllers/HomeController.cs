using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace GymAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;

        public HomeController(DataContext context, ICombosHelper combosHelper, IUserHelper userHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Error 404 method
        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
        public async Task<IActionResult> MyEvent()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.EventAcces.Where(sa => sa.User.Email == User.Identity.Name)
                    .Include(ev => ev.Event)
                    .ToListAsync());
            }

            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> TakeEvent(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int Id = id ?? 1;

                Event evento= await _context.Events.FindAsync(Id);

                TakeEventViewModel model = new()
                {
                    EventId = Id,
                    Events = await _combosHelper.GetComboEventsAsync(),                   
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeService(TakeEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AccessHour == 0)
                {
                    ModelState.AddModelError(string.Empty, "¡Debe seleccionar un horario!");
                    return View(model);
                }

                TimeSpan AccessHour = TimeSpan.FromTicks(model.AccessHour);

                Event evento= await _context.Events.FindAsync(model.EventId);

                EventAcces eventAcces = new()
                {
                    User = await _userHelper.GetUserAsync(User.Identity.Name),
                    Event = evento,
                    AccessDate = model.AccessDate + AccessHour,    
                    EventStatus = Enums.EventsStatus.Pending,
                };

                _context.Add(eventAcces);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyEvent), "Home");
            }

            return View(model);
        }

    }
}