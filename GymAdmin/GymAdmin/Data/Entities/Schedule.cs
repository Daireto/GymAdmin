using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        [Display(Name = "Días")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DayOfWeek Day { get; set; }

        [Display(Name = "Hora inicial")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string StartHour { get; set; }

        [Display(Name = "Hora final")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FinishHour { get; set; }

        public ICollection<Professional> Professionals { get; set; }
    }
}
