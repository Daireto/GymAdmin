using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GymAdmin.Helpers.ModalHelper;

namespace GymAdmin.Controllers
{
    public class PlanController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public PlanController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PlanInscriptions
                .Include(pi => pi.User)
                .Include(pi => pi.Plan)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> CancelPlan(int id)
        {
            try
            {
                PlanInscription planInscription = await _context.PlanInscriptions.FindAsync(id);
                planInscription.PlanStatus = PlanStatus.Cancelled;
                _context.Update(planInscription);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Plan cancelado correctamente", "Operación exitosa:");
            }
            catch
            {
                _flashMessage.Danger("No se pudo cancelar el plan", "Error:");
            }
            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public IActionResult EditActivePlan()
        {
            return View();
        }

        [NoDirectAccess]
        public IActionResult GetPlan()
        {
            return View();
        }
    }
}
