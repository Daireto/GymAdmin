using GymAdmin.Common;
using GymAdmin.Data;
using GymAdmin.Data.Entities;
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
            var allSchedules = GenerateScheduleList(day).Where(s => s.Day.ToString("yyyy-MM-dd") == day.ToString("yyyy-MM-dd"));

            List<ServiceDate> resultSchedules = new();

            foreach (var schedule in allSchedules)
            {
                var serviceAccesses = await _context.ServiceAccesses
                    .Where(sa =>
                        sa.Service.Id == serviceId &&
                        sa.AccessDate == schedule.Day &&
                        sa.ServiceStatus == Enums.ServiceStatus.Pending)
                    .ToListAsync();

                if (serviceAccesses.Count == 0 || serviceAccesses == null)
                {
                    resultSchedules.Add(schedule);
                }
            }

            List<SelectListItem> list = resultSchedules
                .Select(s => new SelectListItem
                {
                    Text = $"{s.Day.TimeOfDay}",
                    Value = $"{s.Day.TimeOfDay.Ticks}"
                })
                .OrderBy(s => s.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione un horario",
                Value = "0"
            });

            return list;
        }

        public List<ServiceDate> GenerateScheduleList(DateTime dayTime)
        {
            List<ServiceDate> allSchedules = new();
            List<DateTime> days = new()
            {
                dayTime,
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
                dayTime.AddDays(1),
            };

            foreach (var day in days)
            {
                for (int i = 7; i <= 11; i++)
                {
                    allSchedules.Add(new ServiceDate
                    {
                        Day = day + new TimeSpan(i, 0, 0)
                    });

                    allSchedules.Add(new ServiceDate
                    {
                        Day = day + new TimeSpan(i, 30, 0)
                    });
                }
                for (int i = 14; i <= 19; i++)
                {
                    allSchedules.Add(new ServiceDate
                    {
                        Day = day + new TimeSpan(i, 0, 0)
                    });

                    allSchedules.Add(new ServiceDate
                    {
                        Day = day + new TimeSpan(i, 30, 0)
                    });
                }
            }

            return allSchedules;
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
    }
}
