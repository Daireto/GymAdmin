using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class RecoverPasswordViewModel
    {
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido")]
        public string Email { get; set; }
    }
}
