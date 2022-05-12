using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Data.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DayOfWeek Day { get; set; }

        [Display(Name = "Hora inicial")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "bigint")]
        public TimeSpan StartHour { get; set; }

        [Display(Name = "Hora final")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "bigint")]
        public TimeSpan FinishHour { get; set; }

        public ICollection<ProfessionalSchedule> ProfessionalSchedules { get; set; }

        public int ProfessionalSchedulesNumber => ProfessionalSchedules == null ? 0 : ProfessionalSchedules.Count;
    }
}
