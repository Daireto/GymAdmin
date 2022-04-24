using GymAdmin.Data;
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
                .OrderBy(p => p.Text)
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un profesional]",
                Value = "0"
            });

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
                .OrderBy(s => s.Text)
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un horario]",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsersAsync()
        {
            List<SelectListItem> list = await _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.Document
                })
                .OrderBy(u => u.Text)
                .ToListAsync();

            return list;
        }
    }
}
