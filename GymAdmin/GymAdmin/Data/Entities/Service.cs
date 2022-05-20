using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Display(Name = "Servicio")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public ICollection<ServiceAccess> ServiceAccesses { get; set; }

        public ICollection<Professional> Professionals { get; set; }

        public int ProfessionalsNumber => Professionals == null ? 0 : Professionals.Count;

        public int AccessesNumber => ServiceAccesses.Where(sa => sa.ServiceStatus == Enums.ServiceStatus.Taken) == null ? 0 : ServiceAccesses.Count;
    }
}
