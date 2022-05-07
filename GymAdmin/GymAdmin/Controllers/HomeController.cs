using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymAdmin.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Instructors()
        {
            return View();
        }

        public IActionResult Physiotherapists()
        {
            return View();
        }

        public IActionResult Nutritionists()
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
    }
}