using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymAdmin.Enums;
namespace GymAdmin.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name = "Dia")]
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

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(600, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Description { get; set; }

        public EventType EventType { get; set; }

        public Director Director { get; set; }

        public ICollection<EventInscription> EventInscriptions { get; set; }

        public int InscriptionsNumber => EventInscriptions == null ? 0 : EventInscriptions.Where(e => e.EventStatus == Enums.EventStatus.SignedUp).Count();

        public ICollection<EventImage> EventImages { get; set; }

        public int ImagesNumber => EventImages == null ? 0 : EventImages.Count();
    }
}
