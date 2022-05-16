using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAdmin.Controllers
{
 
    public class AttendanceController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        public AttendanceController(DataContext context, IUserHelper userHelper)
        {
            _context= context;
            _userHelper = userHelper;
        }


      
        public async Task<IActionResult> Create()
        {

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            PlanInscription pI = await _context.PlanInscriptions.FindAsync(1);
            Plan pl = await _context.Plans.FindAsync(1);
            Attendance at = new Attendance();
            if (pI != null) //the last position is always the last plan that the user bought to access the gym, so we first comprobate if the user has already a plan, then if it is ticketholder and is still active to make the operations
            {
                if (pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.TicketHolder) 
                {

                    at.AttendanceDate = DateTime.Now;
                    at.User = user;
                   
                     pI.RemainingDays -= 1;
                      if (pI.RemainingDays==0) 
                      {
                        pI.PlanStatus = PlanStatus.Cancelled;
                      }
                    
                }
                if (pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.Simple || pI.PlanStatus == PlanStatus.Paid && pI.Plan.PlanType == PlanType.Black)
                {

                    at.AttendanceDate = DateTime.Now;
                    at.User = user;
                    
                    DateTime fech = DateTime.Now;
                    if (fech.Date == pI.ExpirationDate.Date) 
                    {
                        pI.PlanStatus= PlanStatus.Cancelled;
                        
                    }
                }
                _context.Add(at);
                _context.Update(pI);
                await _context.SaveChangesAsync();
                return View(at);
            }
             ModelState.AddModelError(String.Empty,"¡El usuario no cuenta con algun plan activo en el momento!");
            return RedirectToAction("Index", "Home");
        }
    }
}
