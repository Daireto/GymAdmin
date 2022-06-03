using GymAdmin.Data;
using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DataContext _context;

        public DashboardController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int pendingServiceAccesses = await _context.ServiceAccesses
                .Where(sa => sa.ServiceStatus == Enums.ServiceStatus.Pending)
                .CountAsync();

            decimal serviceAccessesIncome = await _context.ServiceAccesses
                .Where(sa => sa.ServiceStatus == Enums.ServiceStatus.Taken)
                .SumAsync(sa => sa.TotalPrice);

            decimal suscriptionsIncome = await _context.PlanInscriptions
                .SumAsync(sa => sa.TotalPrice);

            decimal totalIncome = serviceAccessesIncome + suscriptionsIncome;

            DashboardViewModel model = new()
            {
                Users = await _context.Users.CountAsync(),
                Professionals = await _context.Professionals.CountAsync(),
                Directors = await _context.Directors.CountAsync(),
                PendingContracts = pendingServiceAccesses,
                IncomeByContracts = serviceAccessesIncome,
                IncomeBySuscriptions = suscriptionsIncome,
                TotalIncome = totalIncome,
            };

            return View(model);
        }

        public async Task<JsonResult> DataPieChart()
        {
            List<PieChartModel> list = new();

            var plans = await _context.Plans
                .Include(p => p.PlansInscriptions)
                .ToListAsync();

            foreach (var plan in plans)
            {
                PieChartModel pcm = new()
                {
                    name = plan.Name,
                    y = plan.PlansInscriptionsNumber,
                    sliced = false,
                    selected = false,
                };
                list.Add(pcm);
            }

            return Json(list);
        }

        public async Task<JsonResult> DataBarChart()
        {
            var services = await _context.Services
                .Include(s => s.ServiceAccesses.Where(sa => sa.ServiceStatus == Enums.ServiceStatus.Taken))
                .ToListAsync();

            object[] array = new object[services.Count];

            int i = 0;
            foreach (var service in services)
            {
                array[i] = new object[] { service.Name, service.AccessesNumber };
                i++;
            }

            return Json(array);
        }

        public async Task<JsonResult> EventsDataBarChart()
        {
            var events = await _context.Events
                .Include(e => e.EventInscriptions.Where(ei => ei.EventStatus == Enums.EventStatus.SignedUp))
                .Take(10)
                .ToListAsync();

            events = events.OrderByDescending(e => e.InscriptionsNumber).ToList();

            object[] array = new object[events.Count];

            int i = 0;
            foreach (var eventobject in events)
            {
                array[i] = new object[] { eventobject.Name, eventobject.InscriptionsNumber };
                i++;
            }

            return Json(array);
        }
    }
}
