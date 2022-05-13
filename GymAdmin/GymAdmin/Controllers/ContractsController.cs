﻿using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
    public class ContractsController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public ContractsController(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceAccesses
                .Include(sa => sa.Service)
                .Include(sa => sa.User)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsServiceAccess(int? id)
        {
            return View(await _context.ServiceAccesses
                .Include(sa => sa.Service)
                .Include(sa => sa.User)
                .FirstOrDefaultAsync(sa => sa.Id == id));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetServiceAsTaken(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ServiceAccess serviceAccess = await _context.ServiceAccesses.FindAsync(id);
            if (serviceAccess == null)
            {
                return NotFound();
            }

            if (serviceAccess.ServiceStatus == Enums.ServiceStatus.Pending)
            {
                serviceAccess.ServiceStatus = Enums.ServiceStatus.Taken;
                _context.Update(serviceAccess);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(String.Empty, "¡Sólo se pueden marcar como tomados los servicios pendientes!");
                //TODO: Replace for a flash message
            }

            return RedirectToAction("DetailsServiceAccess", "Contracts", new { id = id });
        }

        public async Task<IActionResult> CancelService(int? id, string route)
        {
            if(id == null)
            {
                return NotFound();
            }

            ServiceAccess serviceAccess = await _context.ServiceAccesses.FindAsync(id);
            if(serviceAccess == null)
            {
                return NotFound();
            }

            if(serviceAccess.ServiceStatus == Enums.ServiceStatus.Pending)
            {
                serviceAccess.ServiceStatus = Enums.ServiceStatus.Cancelled;
                _context.Update(serviceAccess);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(String.Empty, "¡Sólo se pueden cancelar servicios pendientes!");
            }

            if(route == "MyServices")
            {
            return RedirectToAction("MyServices", "Home");
            }
            else
            {
                return RedirectToAction("DetailsServiceAccess", "Contracts", new { id = id });
            }
        }
    }
}