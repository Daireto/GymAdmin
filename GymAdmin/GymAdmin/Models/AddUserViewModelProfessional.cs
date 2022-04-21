using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddUserViewModelProfessional: EditUserProfessionalViewModel
    {
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden")]
        [Display(Name = "Confirmación de contraseña")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Rol")]
        public UserType UserType { get; set; }
        public Service Service { get; set; }
    }
}
