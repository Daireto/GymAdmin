using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Controllers
{
 
    public class AttendanceController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public AttendanceController(DataContext context,ICombosHelper combosHelper)
        {
            _context= context;
            _combosHelper= combosHelper;
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
            if (ModelState.IsValid)
            {
                User user = await _context.Users.Include(u => u.PlanInscriptions).FirstOrDefaultAsync(u => u.Email == model.Username);
                if (user != null)
                {
                    PlanInscription pI = await _context.PlanInscriptions.Include(pI => pI.Plan).FirstOrDefaultAsync(pI => pI.Id == user.PlanInscriptions.Last().Id);

                    Attendance at = new Attendance();
                    if (pI != null && pI.PlanStatus==PlanStatus.Paid) //the last position is always the last plan that the user bought to access the gym, so we first comprobate if the user has already a plan, then if it is ticketholder and is still active to make the operations
                    {
                        if (pI.Plan.PlanType == PlanType.TicketHolder)
                        {

                            at.AttendanceDate = DateTime.Now;
                            at.User = user;

                            pI.RemainingDays -= 1;
                            if (pI.RemainingDays == 0)
                            {
                                pI.PlanStatus = PlanStatus.Cancelled;
                            }

                        }
                        if (pI.Plan.PlanType == PlanType.Simple || pI.Plan.PlanType == PlanType.Black)
                        {

                            at.AttendanceDate = DateTime.Now;
                            at.User = user;

                            DateTime fech = DateTime.Now;
                            if (fech.Date == pI.ExpirationDate.Date)
                            {
                                pI.PlanStatus = PlanStatus.Cancelled;

                            }
                        }
                        _context.Add(at);
                        _context.Update(pI);
                        await _context.SaveChangesAsync();
                        return View(model);
                    }
                    ModelState.AddModelError(String.Empty, "¡El usuario no cuenta con algun plan activo en el momento!");
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(String.Empty, "¡Este usuario no existe en nuestra base de datos!");
                return RedirectToAction("Index", "Home");
            }

            return NotFound();
        }
    }
}
