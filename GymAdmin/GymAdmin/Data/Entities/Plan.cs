using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class Plan
    {

        public int Id { get; set; }

        [Display(Name = "Plan")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]

        public string Name { get; set; }

        [Display(Name = "Precio")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal Price { get; set; }

        public PlanType PlanType { get; set; }

        public ICollection <PlanInscription> PlansInscriptions { get; set;}

    }
}
