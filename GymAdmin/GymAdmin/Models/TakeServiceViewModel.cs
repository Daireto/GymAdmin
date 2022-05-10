using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class TakeServiceViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Servicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ServiceId { get; set; }

        public IEnumerable<SelectListItem> Services { get; set; }

        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DayOfWeek AccessDate { get; set; }

        [Display(Name = "Horarios disponibles")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public long AccessHour { get; set; }

        [Display(Name = "Descuento")]
        public double Discount { get; set; }

        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        [Display(Name = "Profesional")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ProfessionalId { get; set; }
    }
}
