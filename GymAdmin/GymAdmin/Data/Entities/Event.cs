using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name = "dia del evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DayOfWeek Day { get; set; }

        [Display(Name = "Hora del evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string StartHour { get; set; }

        [Display(Name = "Hora final del evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FinishHour { get; set; }

        [Display(Name = "Nombre del evento")]
        public string NameEveneto { get; set; }
        public ICollection<Director> Directors { get; set; }

        public Director Director { get; set; }
    }
}
