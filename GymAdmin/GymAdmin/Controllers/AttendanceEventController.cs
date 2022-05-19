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
    public class AttendanceEventController
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public AttendanceEventController(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            AddAttendanceViewModel model = new AddAttendanceViewModel
            {
                Users = await _combosHelper.GetComboUsersWithEventAsync()
            };

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(AddAttendanceViewModel model)
        {
            string n = model.Username;
            if (ModelState.IsValid)
            {
                User user = await _context.Users.Include(u => u.EventIncriptions).FirstOrDefaultAsync(u => u.Email == model.Username);
                if (user != null)
                {
                    EventIncriptions evento = await _context.EventIncriptions
                        .Include(ev => ev.Event)
                        .FirstOrDefaultAsync(ev => ev.Id == user.EventIncriptions.Last().Id);

                    Attendance at = new Attendance();
                    if (evento != null) //the last position is always the last plan that the user bought to access the gym, so we first comprobate if the user has already a plan, then if it is ticketholder and is still active to make the operations
                    {
                        if (evento.EventsStatus == EventsStatus.Taken && evento.Event.EventType == EventType.Yudo)
                        {

                            at.AttendanceDate = DateTime.Now;
                            at.User = user;


                        }
                        if (evento.EventsStatus == EventsStatus.Taken && evento.Event.EventType == EventType.Boxeo)
                        {

                            at.AttendanceDate = DateTime.Now;
                            at.User = user;
                            if (evento.EventsStatus == EventsStatus.Taken && evento.Event.EventType == EventType.Karate)
                            {

                                at.AttendanceDate = DateTime.Now;
                                at.User = user;

                            }
                            if (evento.EventsStatus == EventsStatus.Taken && evento.Event.EventType == EventType.Rumba)
                            {

                                at.AttendanceDate = DateTime.Now;
                                at.User = user;

                            }
                            if (evento.EventsStatus == EventsStatus.Taken && evento.Event.EventType == EventType.Crostfit)
                            {

                                at.AttendanceDate = DateTime.Now;
                                at.User = user;

                            }
                            _context.Add(at);
                            _context.Update(evento);
                            await _context.SaveChangesAsync();
                            return View(model);
                        }
                        _context.Add(at);
                        _context.Update(evento);
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
