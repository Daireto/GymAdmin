using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using GymAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GymAdmin.Helpers.ModalHelper;
using GymAdmin.Common;

namespace GymAdmin.Controllers
{
    public class PlanController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;

        public PlanController(DataContext context, IFlashMessage flashMessage, ICombosHelper combosHelper, IUserHelper userHelper)
        {
            _context = context;
            _flashMessage = flashMessage;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PlanInscriptions
                .Include(pi => pi.User)
                .Include(pi => pi.Plan)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsPlanInscription(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pi => pi.User)
                .Include(pi => pi.Plan)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (planInscription == null)
            {
                return NotFound();
            }

            return View(planInscription);
        }

        [NoDirectAccess]
        public async Task<IActionResult> CancelPlan(int id)
        {
            PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pi => pi.User)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (planInscription.PlanStatus == PlanStatus.Active)
            {
                try
                {
                    var eventInscriptions = await _context.EventInscriptions
                        .Include(ei => ei.User)
                        .Where(ei => ei.User.Email == planInscription.User.Email)
                        .ToListAsync();

                    if (eventInscriptions != null)
                    {
                        if (eventInscriptions.Count != 0)
                        {
                            foreach (EventInscription eventInscription in eventInscriptions)
                            {
                                eventInscription.EventStatus = EventStatus.Cancelled;
                                _context.Update(eventInscription);
                            }
                        }
                    }

                    planInscription.PlanStatus = PlanStatus.Cancelled;
                    _context.Update(planInscription);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Plan cancelado correctamente", "Operación exitosa:");
                }
                catch
                {
                    _flashMessage.Danger("No se pudo cancelar el plan", "Error:");
                }
            }
            else
            {
                _flashMessage.Danger("Sólo se pueden cancelar planes activos", "Error:");
            }
            return RedirectToAction(nameof(DetailsPlanInscription), new { id = id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> GetPlan()
        {
            GetPlanViewModel model = new()
            {
                Plans = await _combosHelper.GetComboPlansAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPlan(GetPlanViewModel model)
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);

            PlanInscription planInscription = await _context.PlanInscriptions
                .Include(pI => pI.User)
                .Include(pI => pI.Plan)
                .FirstOrDefaultAsync(
                    pI => pI.User.UserName == User.Identity.Name &&
                    pI.PlanStatus == PlanStatus.Active
                );

            if (planInscription == null)
            {
                Plan plan = await _context.Plans.FindAsync(model.PlanId);

                if(plan == null)
                {
                    _flashMessage.Warning("El plan seleccionado no existe", "Error:");
                    model.Plans = await _combosHelper.GetComboPlansAsync();
                    return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "GetPlan", model) });
                }

                try
                {
                    if (plan.PlanType == PlanType.Regular || plan.PlanType == PlanType.Black)
                    {
                        if (model.Duration < 1)
                        {
                            _flashMessage.Warning("La duración no es válida", "Error:");
                            model.Plans = await _combosHelper.GetComboPlansAsync();
                            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "GetPlan", model) });
                        }
                        else if (model.Duration > 12)
                        {
                            _flashMessage.Warning("Sólo se permiten hasta 12 meses de duración por suscripción", "Error:");
                            model.Plans = await _combosHelper.GetComboPlansAsync();
                            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "GetPlan", model) });
                        }

                        PlanInscription pI = new()
                        {
                            ActivationDate = DateTime.Today,
                            ExpirationDate = DateTime.Today.AddDays(model.Duration * 30),
                            InscriptionDate = DateTime.Today,
                            PlanStatus = PlanStatus.Active,
                            Plan = plan,
                            User = user,
                            Duration = 30 * model.Duration,
                            RemainingDays = model.Duration,
                            Discount = 0,
                            TotalPrice = plan.Price * model.Duration,
                        };
                        _context.Add(pI);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (model.Duration < 1)
                        {
                            _flashMessage.Warning("La cantidad de tiquetes no es válida", "Error:");
                            model.Plans = await _combosHelper.GetComboPlansAsync();
                            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "GetPlan", model) });
                        }
                        else if (model.Duration > 12)
                        {
                            _flashMessage.Warning("Sólo se permiten hasta 12 tiquetes por usuario", "Error:");
                            model.Plans = await _combosHelper.GetComboPlansAsync();
                            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "GetPlan", model) });
                        }

                        PlanInscription pI2 = new PlanInscription
                        {
                            ActivationDate = DateTime.Today,
                            ExpirationDate = DateTime.Today.AddDays(90),
                            InscriptionDate = DateTime.Today,
                            PlanStatus = PlanStatus.Active,
                            Plan = plan,
                            User = user,
                            Duration = model.Duration,
                            RemainingDays = model.Duration,
                            Discount = 0,
                            TotalPrice = plan.Price * model.Duration
                        };
                        _context.Add(pI2);
                        await _context.SaveChangesAsync();
                    }
                    _flashMessage.Confirmation("Suscripción realizada correctamente", "Operación exitosa:");
                    return Json(new { isValid = true, redirect = true, route = "Account/ViewUser" });
                }
                catch
                {
                    _flashMessage.Warning("No se pudo realizar la suscripción, si el problema persiste comunícate con soporte", "Error:");
                    return Json(new { isValid = true, redirect = true, route = "Account/ViewUser" });
                }
            }
            else
            {
                _flashMessage.Warning("Ya estás suscrito a un plan", "Error:");
                return Json(new { isValid = true, redirect = true, route = "Account/ViewUser" });
            }
        }

        [NoDirectAccess]
        public JsonResult GetDurationLabel(int planId)
        {
            Plan plan = _context.Plans.Find(planId);
            string Label;

            if(plan == null)
            {
                return Json(new { value = "" });
            }

            if (plan.PlanType == PlanType.TicketHolder)
            {
                Label = "Cantidad de tiquetes";
            }
            else
            {
                Label = "Duración en meses";
            }

            return Json(new { value = Label });
        }

        [NoDirectAccess]
        public JsonResult GetPrice(int planId, int duration)
        {
            Plan plan = _context.Plans.Find(planId);

            if (plan == null)
            {
                return Json(new { priceValue = "", totalPriceValue = "" });
            }

            string p = $"{plan.Price:C2}";

            decimal totalPrice = plan.Price * duration;
            string tp = $"{totalPrice:C2}";

            return Json(new { priceValue = p, totalPriceValue = tp });
        }
    }
}
