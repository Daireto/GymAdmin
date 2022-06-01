﻿using GymAdmin.Data;
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

        public PlanController(DataContext context, IFlashMessage flashMessage, ICombosHelper combosHelper )
        {
            _context = context;
            _flashMessage = flashMessage;
            _combosHelper = combosHelper;
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
        public IActionResult EditActivePlan()
        {
            return View();
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

        public async Task<IActionResult> GetPlan(GetPlanViewModel model)
        {
            User user = await _context.Users.Include(u => u.PlanInscriptions).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            PlanInscription pI = user.PlanInscriptions.Last();
            if (pI.PlanStatus != PlanStatus.Active || pI == null)
            {
                Plan plan = await _context.Plans.FindAsync(model.PlanId);
                if (plan.PlanType == PlanType.Regular || plan.PlanType == PlanType.Black) {

                    PlanInscription pI2 = new PlanInscription
                    {
                        ActivationDate = DateTime.Now,
                        ExpirationDate = DateTime.Now.AddDays(model.Duration),
                        InscriptionDate = DateTime.Now,
                        PlanStatus = PlanStatus.Active,
                        Plan = plan,
                        User = user,
                        Duration = model.Duration,
                        RemainingDays = model.Duration,
                        Discount = DiscountValues.GetDiscountValue(plan.PlanType),
                        TotalPrice = plan.Price

                    };
                    user.PlanInscriptions.Add(pI2);
                    _context.Add(pI2);
                    await _context.SaveChangesAsync();
                }
                else
                { 
                    PlanInscription pI2 = new PlanInscription
                    {
                        ActivationDate = DateTime.Now,
                        ExpirationDate = DateTime.Now.AddDays(90), //the plan ticketholder will have 3 months to be used, the other way it will be expired
                        InscriptionDate = DateTime.Now,
                        PlanStatus = PlanStatus.Active,
                        Plan = plan,
                        User = user,
                        Duration = model.Duration,
                        RemainingDays = model.Duration,
                        Discount = DiscountValues.GetDiscountValue(plan.PlanType),
                        TotalPrice = plan.Price
                    };
                    user.PlanInscriptions.Add(pI2);
                    _context.Add(pI2);
                    await _context.SaveChangesAsync();

                }
                return View(model);



            }
            else { 
             //the user cannot suscribe to another plan because has another one active at the moment
            }

            
        }


    }
}
