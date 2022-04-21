using Microsoft.AspNetCore.Mvc;
using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GymAdmin.Controllers
{
   
public class ServiceController : Controller
{
            private readonly DataContext _context;
            private readonly IUserHelperProfessional _userProfessionalHelper;
            private readonly IBlobHelper _blobHelper;
            private readonly IMailHelper _mailHelper;

            public ServiceController(DataContext context, IUserHelperProfessional userProfessionalHelper, IBlobHelper blobHelper, IMailHelper mailHelper)
            {
                _context = context;
            _userProfessionalHelper = userProfessionalHelper;
                _blobHelper = blobHelper;
                _mailHelper = mailHelper;
            }

            // GET: Services
            public async Task<IActionResult> Index()
            {
                return View(await _context.Services.ToListAsync());
            }

            // GET: Services/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Services
                    .Include(s => s.Professionals)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (service == null)
                {
                    return NotFound();
                }

                return View(service);
            }

            // GET: Services/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Services/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Service service)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(service);
            }

            // GET: Services/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound();
                }
                return View(service);
            }

            // POST: Services/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Service service)
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
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ServiceExists(service.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(service);
            }

            // GET: Services/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Services
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (service == null)
                {
                    return NotFound();
                }

                return View(service);
            }

            // POST: Services/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var service = await _context.Services.FindAsync(id);
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool ServiceExists(int id)
            {
                return _context.Services.Any(e => e.Id == id);
            }


            public IActionResult CreateProfessional()
            {
                AddUserViewModel model = new()
                {
                    Id = Guid.Empty.ToString(),
                    UserType = UserType.Admin
                };

                return View(model);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateProfessional(AddUserViewModelProfessional model)
            {
                if (ModelState.IsValid)
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }

                    UserProfessional user = await _userProfessionalHelper.AddUserAsync(model, imageId);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "¡Este correo ya está en uso!");
                        return View(model);
                    }

                    //Email confirmation
                    string token = await _userProfessionalHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new
                        {
                            UserId = user.Id,
                            Token = token
                        },
                        protocol: HttpContext.Request.Scheme);

                    string body = "<style>body{text-align:center;font-family:Verdana,Arial;}</style>" +
                        $"<h1>Soporte GymAdmin</h1>" +
                        $"<h3>Estás a un solo paso de ser parte de nuestra comunidad de Profesionales</h3>" +
                        $"<h4>Sólo debes hacer click en el siguiente botón para confirmar tu email</h4>" +
                        $"<br/>" +
                        $"<a style=\"padding:15px;background-color:#f1b00e;text-decoration:none;color:black;border: 5px solid #000;border-radius:10px;\" href=\"{tokenLink}\">Confirmar email</a>";

                    Response response = _mailHelper.SendMail(
                        user.FullName,
                        model.Username,
                        "GymAdmin - Confirmación del email",
                        body);

                    if (response.IsSuccess)
                    {
                        return RedirectToAction("ConfirmEmailMessage", "Account");
                    }
                    else
                    {
                        return RedirectToAction("ConfirmEmailErrorMessage", "Account");
                    }
                }
                return View(model);
            }
        }
}
