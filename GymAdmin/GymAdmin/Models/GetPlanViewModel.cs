using GymAdmin.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class GetPlanViewModel
    {
        [Display(Name = "Plan")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int PlanId { get; set; }

        public IEnumerable<SelectListItem> Plans { get; set; }

        [Display(Name = "Duración")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Duration { get; set; }
    }
}
