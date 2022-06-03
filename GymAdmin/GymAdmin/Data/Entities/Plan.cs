using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public PlanType PlanType { get; set; }

        public ICollection <PlanInscription> PlansInscriptions { get; set;}

        public int PlansInscriptionsNumber => PlansInscriptions == null ? 0 : PlansInscriptions.Count;
    }
}
