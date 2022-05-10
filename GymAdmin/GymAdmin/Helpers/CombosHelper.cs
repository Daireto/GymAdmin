﻿using GymAdmin.Common;
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

        public async Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync(int serviceId, DayOfWeek day)
        {
            var allSchedules = GenerateScheduleList().Where(s => s.Day == day);

            List<ServiceDate> resultSchedules = new();

            foreach (var schedule in allSchedules)
            {
                var serviceAccesses = await _context.ServiceAccesses
                    .Where(sa =>
                        sa.Service.Id == serviceId &&
                        sa.AccessDate == schedule.Day &&
                        sa.AccessHour == schedule.AccessHour &&
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
                    Text = $"{s.AccessHour}",
                    Value = $"{s.AccessHour.Ticks}"
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

        public List<ServiceDate> GenerateScheduleList()
        {
            List<ServiceDate> allSchedules = new();
            List<DayOfWeek> days = new()
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday,
            };

            foreach (var day in days)
            {
                for (int i = 7; i <= 11; i++)
                {
                    allSchedules.Add(new ServiceDate
                    {
                        Day = day,
                        AccessHour = new TimeSpan(i, 0, 0)
                    });

                    allSchedules.Add(new ServiceDate
                    {
                        Day = day,
                        AccessHour = new TimeSpan(i, 30, 0)
                    });
                }
                for (int i = 14; i <= 19; i++)
                {
                    allSchedules.Add(new ServiceDate
                    {
                        Day = day,
                        AccessHour = new TimeSpan(i, 0, 0)
                    });

                    allSchedules.Add(new ServiceDate
                    {
                        Day = day,
                        AccessHour = new TimeSpan(i, 30, 0)
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
