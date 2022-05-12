using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastName { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Document { get; set; }

        [Display(Name = "Tipo de documento")]
        public DocumentType DocumentType { get; set; }

        [Display(Name = "Foto")]
        public Guid ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://localhost:7156/images/noimage.png" //TODO: Correct path
            : $"https://gymadmin1.blob.core.windows.net/users/{ImageId}";

        [Display(Name = "Foto")]
        public IFormFile? ImageFile { get; set; }
    }
}