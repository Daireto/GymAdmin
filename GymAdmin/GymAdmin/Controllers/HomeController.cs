﻿using GymAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymAdmin.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        //TODO: Try the project with and without this

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
    }
}