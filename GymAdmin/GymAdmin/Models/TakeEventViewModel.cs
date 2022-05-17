using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class TakeEventViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Evento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int EventId { get; set; }

        public IEnumerable<SelectListItem> Events { get; set; }
        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime AccessDate { get; set; }

        [Display(Name = "Horarios disponibles")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public long AccessHour { get; set; }

        [Display(Name = "Director")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int DirectorId { get; set; }

    }
}
