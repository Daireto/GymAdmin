using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddServiceViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Servicio")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal Price { get; set; }

        [Display(Name = "Profesional")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string ProfessionalUserName { get; set; }

        public IEnumerable<SelectListItem> Professionals { get; set; }
    }
}
