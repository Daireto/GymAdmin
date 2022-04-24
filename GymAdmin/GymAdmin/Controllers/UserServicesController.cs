using GymAdmin.Data;
using GymAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
    public class UserServicesController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public UserServicesController(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.ServiceAccesses.FirstOrDefaultAsync(sa => sa.User.UserName == User.Identity.Name));
            }
            return RedirectToAction("Index", "Home");
        }

        //TODO: Pending to create the method to take a service
    }
}
