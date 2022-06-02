using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vereyon.Web;
using static GymAdmin.Helpers.ModalHelper;

namespace GymAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public HomeController(DataContext context, ICombosHelper combosHelper, IUserHelper userHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        //------------------------------------- Principal --------------------------------------------
        public async Task<IActionResult> Index()
        {
            ICollection<Event> events = await _context.Events
                .Include(e => e.Director)
                .ThenInclude(d => d.User)
                .ToListAsync();

            var MondayNumber = events.Where(e => e.Day == DayOfWeek.Monday).Count();
            var TuesdayNumber = events.Where(e => e.Day == DayOfWeek.Tuesday).Count();
            var WednesdayNumber = events.Where(e => e.Day == DayOfWeek.Wednesday).Count();
            var ThursdayNumber = events.Where(e => e.Day == DayOfWeek.Thursday).Count();
            var FridayNumber = events.Where(e => e.Day == DayOfWeek.Friday).Count();
            var SaturdayNumber = events.Where(e => e.Day == DayOfWeek.Saturday).Count();
            var SundayNumber = events.Where(e => e.Day == DayOfWeek.Sunday).Count();

            List<int> list = new()
            {
                MondayNumber, TuesdayNumber, WednesdayNumber, ThursdayNumber, FridayNumber, SaturdayNumber, SundayNumber
            };

            int rows = list.Max();

            HomeIndexEventsViewModel model = new()
            {
                MondayEvents = events.Where(e => e.Day == DayOfWeek.Monday).OrderBy(e => e.StartHour.Hours).ToList(),
                TuesdayEvents = events.Where(e => e.Day == DayOfWeek.Tuesday).OrderBy(e => e.StartHour.Hours).ToList(),
                WednesdayEvents = events.Where(e => e.Day == DayOfWeek.Wednesday).OrderBy(e => e.StartHour.Hours).ToList(),
                ThursdayEvents = events.Where(e => e.Day == DayOfWeek.Thursday).OrderBy(e => e.StartHour.Hours).ToList(),
                FridayEvents = events.Where(e => e.Day == DayOfWeek.Friday).OrderBy(e => e.StartHour.Hours).ToList(),
                SaturdayEvents = events.Where(e => e.Day == DayOfWeek.Saturday).OrderBy(e => e.StartHour.Hours).ToList(),
                SundayEvents = events.Where(e => e.Day == DayOfWeek.Sunday).OrderBy(e => e.StartHour.Hours).ToList(),
                MondayNumber = rows - MondayNumber,
                TuesdayNumber = rows - TuesdayNumber,
                WednesdayNumber = rows - WednesdayNumber,
                ThursdayNumber = rows - ThursdayNumber,
                FridayNumber = rows - FridayNumber,
                SaturdayNumber = rows - SaturdayNumber,
                SundayNumber = rows - SundayNumber,
                RowsNumber = list.Max()
            };

            return View(model);
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Professionals()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //------------------------------------- User --------------------------------------------
        public async Task<IActionResult> MyPlan()
        {
            User user = await _context.Users.Include(u => u.PlanInscriptions).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pI => pI.User)
                .Include(pI => pI.Plan)
                .FirstOrDefaultAsync(
                    pI => pI.User.UserName == User.Identity.Name &&
                    pI.PlanStatus == PlanStatus.Active
                );

            if (planInscription == null)
            {
                return View(nameof(NoPlan));
            }

            return View(planInscription);
        }

        [NoDirectAccess]
        public IActionResult NoPlan()
        {
            return View();
        }

        public async Task<IActionResult> MyEvents()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.EventInscriptions.Where(ei => ei.User.Email == User.Identity.Name)
                    .Include(ei => ei.Event)
                    .ThenInclude(e => e.Director)
                    .ThenInclude(d => d.User)
                    .ToListAsync());
            }

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> MyServices()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.ServiceAccesses.Where(sa => sa.User.Email == User.Identity.Name)
                    .Include(sa => sa.Service)
                    .ToListAsync());
            }

            return RedirectToAction("Login", "Account");
        }

        [NoDirectAccess]
        public async Task<IActionResult> TakeService(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int Id = id ?? 1;

                Service service = await _context.Services.FindAsync(Id);

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

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
                        _flashMessage.Danger("No tienes ningún plan activo, debes adquirir uno para acceder a un servicio", "Error:");
                        return RedirectToAction("ViewUser", "Account");
                    }
                    else
                    {
                        return RedirectToAction("ViewUser", "Account");
                    }
                }

                TakeServiceViewModel model = new()
                {
                    ServiceId = Id,
                    Services = await _combosHelper.GetComboServicesAsync(),
                    Discount = DiscountValues.GetDiscountValue(planInscription.Plan.PlanType),
                    Price = service.Price
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
        public async Task<IActionResult> TakeService(TakeServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AccessHour == 0)
                {
                    model.Services = await _combosHelper.GetComboServicesAsync();
                    _flashMessage.Danger("Debe seleccionar un horario", "Error:");
                    return View(model);
                }

                TimeSpan AccessHour = TimeSpan.FromTicks(model.AccessHour);

                Service service = await _context.Services.FindAsync(model.ServiceId);

                ServiceAccess serviceAccess = new()
                {
                    User = await _userHelper.GetUserAsync(User.Identity.Name),
                    Service = service,
                    AccessDate = model.AccessDate + AccessHour,
                    Discount = model.Discount,
                    TotalPrice = service.Price - (service.Price * (decimal)model.Discount),
                    ServiceStatus = Enums.ServiceStatus.Pending,
                };

                _context.Add(serviceAccess);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Servicio contratado correctamente", "Operación exitosa:");
                return RedirectToAction(nameof(MyServices));
            }

            return View(model);
        }

        [NoDirectAccess]
        public async Task<JsonResult> GetHours(int serviceId, DateTime day)
        {
            var list = await _combosHelper.GetComboSchedulesAsync(serviceId, day);
            return Json(list);
        }

        [NoDirectAccess]
        public async Task<JsonResult> GetPrice(int serviceId)
        {
            Service service = _context.Services.Find(serviceId);

            PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pI => pI.User)
                .Include(pI => pI.Plan)
                .FirstOrDefaultAsync(
                    pI => pI.User.UserName == User.Identity.Name &&
                    pI.PlanStatus == PlanStatus.Active
                );

            string p = $"{service.Price:C2}";

            decimal totalPrice = (decimal)(Decimal.ToDouble(service.Price) - (Decimal.ToDouble(service.Price) * DiscountValues.GetDiscountValue(planInscription.Plan.PlanType)));
            string tp = $"{totalPrice:C2}";

            return Json(new { priceValue = p, totalPriceValue = tp });
        }

        //------------------------------------- Errors --------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}