using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboServicesAsync()
        {
            List<SelectListItem> list = await _context.Services
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                })
                .ToListAsync();

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync(int serviceId, DateTime day)
        {
            List<DateTime> allSchedules = new();

            for(int i=7; i<=18; i++)
            {
                if(i != 12 && i != 13)
                {
                    allSchedules.Add(day + new TimeSpan(i, 0, 0));
                }
            }

            List<DateTime> resultSchedules = new();

            foreach (var schedule in allSchedules)
            {
                var serviceAccesses = await _context.ServiceAccesses
                    .Where(sa =>
                        sa.Service.Id == serviceId &&
                        sa.AccessDate == schedule &&
                        sa.ServiceStatus == Enums.ServiceStatus.Pending)
                    .ToListAsync();

                if (serviceAccesses.Count == 0 || serviceAccesses == null)
                {
                    var professionals = await _context.Professionals
                        .Include(p => p.Service)
                        .Include(p => p.ProfessionalSchedules)
                        .ThenInclude(ps => ps.Schedule)
                        .Where(p => p.Service.Id == serviceId)
                        .ToListAsync();

                    bool result = professionals.Any(p => p.ProfessionalSchedules.Any(ps =>
                        ps.Schedule.Day == schedule.DayOfWeek &&
                        ps.Schedule.StartHour.Ticks <= schedule.TimeOfDay.Ticks &&
                        ps.Schedule.FinishHour.Ticks > schedule.TimeOfDay.Ticks));

                    if (result)
                    {
                        resultSchedules.Add(schedule);
                    }
                }
            }

            List<SelectListItem> list = resultSchedules
                .Select(s => new SelectListItem
                {
                    Text = $"{s.ToString("hh:mm tt")}",
                    Value = $"{s.TimeOfDay.Ticks}"
                })
                .OrderBy(s => s.Text)
                .OrderBy(s => s.Text.Substring(s.Text.Length - 5, 5))
                .ToList();

            if (list.Count != 0)
            {
                list.Insert(0, new SelectListItem
                {
                    Text = "Seleccione un horario",
                    Value = "0"
                });
            }
            else
            {
                list.Insert(0, new SelectListItem
                {
                    Text = "No hay profesionales disponibles en este día",
                    Value = "0"
                });
            }

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsersAsync()
        {
            List<Professional> professionals = await _context.Professionals
                .Include(p => p.User)
                .ToListAsync();
            List<string> userIds = new();
            foreach (var professional in professionals)
            {
                userIds.Add(professional.User.Id);
            }

            List<SelectListItem> list = await _context.Users
                .Where(u => !userIds.Contains(u.Id))
                .Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.UserName
                })
                .ToListAsync();

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsersWithPlanAsync()
        {
            List<SelectListItem> list = await _context.PlanInscriptions
                .Include(pI => pI.User)
                .Include(pI => pI.Plan)
                .Where(pI => pI.PlanStatus == PlanStatus.Active)
                .Select(pI => new SelectListItem
                {
                    Text = pI.User.FullName,
                    Value = pI.User.UserName
                })
                .ToListAsync();

            return list;
        }
    }
}
