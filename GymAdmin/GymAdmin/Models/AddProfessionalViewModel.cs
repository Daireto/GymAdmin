using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddProfessionalViewModel : AddUserViewModel
    {
        [Display(Name = "Horario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ScheduleId { get; set; }

        public IEnumerable<SelectListItem> Schedules { get; set; }

        [Display(Name = "Profesión")]
        public ProfessionalType ProfessionalType { get; set; }
    }
}
