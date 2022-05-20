using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
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

        public IActionResult Index()
        {
            return View();
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

                TakeServiceViewModel model = new()
                {
                    ServiceId = Id,
                    Services = await _combosHelper.GetComboServicesAsync(),
                    Discount = DiscountValues.GetDiscountValue("Regular"),
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
                _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
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
        public JsonResult GetPrice(int serviceId)
        {
            Service service = _context.Services.Find(serviceId);

            string p = $"{service.Price:C2}";

            decimal totalPrice = (decimal)(Decimal.ToDouble(service.Price) - (Decimal.ToDouble(service.Price) * DiscountValues.GetDiscountValue("Regular")));
            string tp = $"{totalPrice:C2}";

            return Json(new { priceValue = p, totalPriceValue = tp });
        }

        //TODO: Add events and plans methods

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