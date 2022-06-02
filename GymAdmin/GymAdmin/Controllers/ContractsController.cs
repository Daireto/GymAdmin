using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GymAdmin.Helpers.ModalHelper;

namespace GymAdmin.Controllers
{
    public class ContractsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public ContractsController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
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
            if (id == null)
            {
                return NotFound();
            }

            ServiceAccess serviceAccess = await _context.ServiceAccesses
                .Include(sa => sa.Service)
                .Include(sa => sa.User)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (serviceAccess == null)
            {
                return NotFound();
            }

            return View(serviceAccess);
        }

        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> SetServiceAsTaken(int id)
        {
            ServiceAccess serviceAccess = await _context.ServiceAccesses.FindAsync(id);

            if (serviceAccess.ServiceStatus == Enums.ServiceStatus.Pending)
            {
                serviceAccess.ServiceStatus = Enums.ServiceStatus.Taken;
                _context.Update(serviceAccess);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Servicio actualizado correctamente", "Operación exitosa:");
            }
            else
            {
                _flashMessage.Danger("Sólo se pueden marcar como tomados los servicios pendientes", "Error:");
            }

            return RedirectToAction("DetailsServiceAccess", "Contracts", new { id = id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> CancelService(int id)
        {
            ServiceAccess serviceAccess = await _context.ServiceAccesses.FindAsync(id);

            if (serviceAccess.ServiceStatus == Enums.ServiceStatus.Pending)
            {
                serviceAccess.ServiceStatus = Enums.ServiceStatus.Cancelled;
                _context.Update(serviceAccess);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Servicio cancelado correctamente", "Operación exitosa:");
            }
            else
            {
                _flashMessage.Danger("Sólo se pueden cancelar servicios pendientes", "Error:");
            }

            if (User.IsInRole("User"))
            {
                return RedirectToAction("MyServices", "Home");
            }

            return RedirectToAction("DetailsServiceAccess", "Contracts", new { id = id });
        }
    }
}
