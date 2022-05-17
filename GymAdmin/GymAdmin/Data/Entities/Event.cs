using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName="bigint")]
        public TimeSpan StartHour { get; set; }

        [Display(Name = "Hora final del evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "bigint")]
        public TimeSpan FinishHour { get; set; }

        [Display(Name = "Nombre del evento")]
        public string NameEveneto { get; set; }
        //public ICollection<EventAcces> EventAcces { get; set; }

        public Director Director { get; set; }
    }
}
