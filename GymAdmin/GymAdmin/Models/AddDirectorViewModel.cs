using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddDirectorViewModel : AddUserViewModel
    {
        [Display(Name = "Evento horario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int EventId { get; set; }
        public IEnumerable<SelectListItem> Events { get; set; }

        [Display(Name = "Tipo de evento")]
        public EventType EventType { get; set; }
    
    }
}
