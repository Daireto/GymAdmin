using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class EditProfessionalViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Username { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        [Display(Name = "Profesión")]
        public ProfessionalType ProfessionalType { get; set; }
    }
}
