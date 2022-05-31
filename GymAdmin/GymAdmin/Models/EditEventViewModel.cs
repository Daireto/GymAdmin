using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Models
{
    public class EditEventViewModel
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

        [Display(Name = "Nombre del evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(600, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Description { get; set; }

        [Display(Name = "Tipo de evento")]
        public EventType EventType { get; set; }
    }
}
