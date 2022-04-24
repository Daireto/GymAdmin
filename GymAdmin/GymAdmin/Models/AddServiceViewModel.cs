using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Models
{
    public class AddServiceViewModel
    {
        [Display(Name = "Servicio")]
        [MinLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        [MinLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal Price { get; set; }

        [Display(Name = "Profesional")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ProfessionalId { get; set; }

        public IEnumerable<SelectListItem> Professionals { get; set; }
    }
}
