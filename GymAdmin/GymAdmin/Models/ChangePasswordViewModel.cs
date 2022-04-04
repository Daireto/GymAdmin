using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña nueva")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y la confirmación no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de contraseña nueva")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string ConfirmPassword { get; set; }
    }
}
