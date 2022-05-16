using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymAdmin.Models
{
    public class AddAttendanceViewModel
    {
        public int Id { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }

        public string Username { get; set;}
    }
}
