using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class EditDirectorViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Username { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        [Display(Name = "Horario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ScheduleId { get; set; }

        public IEnumerable<SelectListItem> Events { get; set; }

        [Display(Name = "Profesión")]
        public DirectorType DirectorType { get; set; }
        public int EventId { get; set; }
    }
}
