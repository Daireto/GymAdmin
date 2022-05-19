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

        public async Task<IEnumerable<SelectListItem>> GetComboProfessionalsAsync()
        {
            List<SelectListItem> list = await _context.Professionals
                .Select(p => new SelectListItem
                {
                    Text = p.User.FullName,
                    Value = p.User.UserName
                })
                .ToListAsync();

            return list;
        }
        public async Task<IEnumerable<SelectListItem>> GetComboDirectorsAsync()
        {
            List<SelectListItem> list = await _context.Directors
                .Select(p => new SelectListItem
                {
                    Text = p.User.FullName,
                    Value = p.User.UserName
                })
                .ToListAsync();

            return list;
        }
        public async Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync()
        {
            List<SelectListItem> list = await _context.Schedules
                .Select(s => new SelectListItem
                {
                    Text = $"{s.Day}, {s.StartHour} - {s.FinishHour}",
                    Value = $"{s.Id}"
                })
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione un horario",
                Value = "0"
            });

            return list;
        }
        /*
        public async Task<IEnumerable<SelectListItem>> GetComboEventAsync()
        {
            List<SelectListItem> list = await _context.Events
                .Select(s => new SelectListItem
                {
                    Text = $"{s.Day}, {s.StartHour} - {s.FinishHour}",
                    Value = $"{s.Id}"
                })
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione un horario para el evento",
                Value = "0"
            });

            return list;
        }
        */
        public async Task<IEnumerable<SelectListItem>> GetComboUsersAsync()
        {
            List<Professional> professionals = await _context.Professionals
                .Include(p => p.User)
                .ToListAsync();
            List<string> userIds = new();
            foreach(var professional in professionals)
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

        public async Task<IEnumerable<SelectListItem>> GetComboEventsAsync()
        {
            List<SelectListItem> list = await _context.Events
               .Select(s => new SelectListItem
               {
                   Text = $"{s.Day}, {s.StartHour} - {s.FinishHour}",
                   Value = $"{s.Id}"
               })
               .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione un horario para el evento",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsersWithEventAsync()
        {
            List<SelectListItem> list = await _context.Users
                .Where(u => u.EventIncriptions.LastOrDefault() != null)
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
