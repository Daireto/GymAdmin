using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GymAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        //Principal pages
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Gallery()
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

        //Services, events and plans
        public async Task<IActionResult> TakeService()
        {
            if (User.Identity.IsAuthenticated)
            {
                IEnumerable<Service> services = await _context.Services
                        .Include(s => s.Professionals)
                        .ThenInclude(p => p.ProfessionalSchedules)
                        .ThenInclude(ps => ps.Schedule)
                        .ToListAsync();

                return View(services);
            }
            else
            {
                ViewBag.Message = "TakeService";
                return RedirectToAction("Account", "Login");
            }
        }
        
        //TODO: Add events and plans methods

        //Error control
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
    }
}