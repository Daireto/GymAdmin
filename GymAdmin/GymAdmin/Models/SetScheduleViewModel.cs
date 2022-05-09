using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class SetScheduleViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DayOfWeek Day { get; set; }

        [Display(Name = "Hora inicial")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public TimeSpan StartHour { get; set; }

        [Display(Name = "Hora final")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public TimeSpan FinishHour { get; set; }

        public int? ProfessionalId { get; set; }
    }
}
