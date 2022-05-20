using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;

namespace GymAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AttendanceController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IFlashMessage _flashMessage;

        public AttendanceController(DataContext context, ICombosHelper combosHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Attendances
                .Include(a => a.User)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            AddAttendanceViewModel model = new AddAttendanceViewModel
            {
                Users = await _combosHelper.GetComboUsersWithPlanAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAttendanceViewModel model)
        {
            string n = model.Username;
            if (ModelState.IsValid)
            {
                User user = await _context.Users.Include(u => u.PlanInscriptions).FirstOrDefaultAsync(u => u.Email == model.Username);
                if (user != null)
                {
                    PlanInscription pI = await _context.PlanInscriptions.Include(pI => pI.Plan).FirstOrDefaultAsync(pI => pI.Id == user.PlanInscriptions.Last().Id);

                    Attendance at = new Attendance();
                    if (pI != null)
                    {
                        at.AttendanceDate = DateTime.Now;
                        at.User = user;
                        if (pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.TicketHolder)
                        {
                            pI.RemainingDays -= 1;
                            if (pI.RemainingDays == 0)
                            {
                                pI.PlanStatus = PlanStatus.Cancelled;
                            }
                        }
                        if (pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.Simple || pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.Black)
                        {
                            DateTime fech = DateTime.Now;
                            if (fech.Date == pI.ExpirationDate.Date)
                            {
                                pI.PlanStatus = PlanStatus.Cancelled;
                            }
                        }
                        _context.Add(at);
                        _context.Update(pI);
                        await _context.SaveChangesAsync();
                        _flashMessage.Confirmation("Registro insertado correctamente", "Operación exitosa:");
                        return Json(new
                        {
                            isValid = true,
                            html = ModalHelper.RenderRazorViewToString(this, "_ViewAllAttendances", _context.Attendances
                                    .Include(a => a.User)
                                    .ToList())
                        });
                    }
                    _flashMessage.Danger("El usuario no cuenta con algun plan activo en el momento", "Error:");
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
                }
                _flashMessage.Danger("Este usuario no existe en nuestra base de datos", "Error:");
            }
            model.Users = await _combosHelper.GetComboUsersWithPlanAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }
    }
}
