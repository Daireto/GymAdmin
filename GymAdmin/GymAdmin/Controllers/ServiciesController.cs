using GymAdmin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiciesController : Controller
    {
        private readonly DataContext _context;

        public ServiciesController(DataContext context)
        {
            _context = context;
        }

        //Index slope
        public async Task<IActionResult> Index()
        {
            return View();
            /*
            return View(await _context.Service
                .Include(s => s.Servicies)
                .ToListAsync());
            */
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Servicies
                .Include(s => s.Servicies)
                .ThenInclude(p => p.Professional)
                .thenInclude(sc => sc.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service== null)
            {
                return NotFound();
            }

            return View(service);
        }
        public IActionResult Create()
        {
            Service servicie = new() { Professional = new List<State>(), Schedule = new List<Schedule>() };
            return View(servicie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelService.IsValid)
            {
                try
                {
                    _context.Add(servicie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelService.AddModelError(string.Empty, "Ya existe un Servicio con el mismo nombre.");
                    }
                    else
                    {
                        ModelService.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(service);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Service
                .Include(s => s.Servicies)
                .ThenInclude(p => p.Professional)
                .thenInclude(sc => sc.Schedule)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelService.AddModelError(string.Empty, "Ya existe un Servicio con el mismo nombre.");
                    }
                    else
                    {
                        ModelService.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(service);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Service service = await _context.Service
               .Include(s => s.Servicies)
               .ThenInclude(p => p.Professional)
               .thenInclude(sc => sc.Schedule)
               .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Service service = await _context.Servicie.FindAsync(id);
            _context.Countries.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
