﻿using GymAdmin.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class User : IdentityUser
    {
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
            ? $"https://gymadmin1.azurewebsites.net/images/noimage.png"
            : $"https://gymadmin1.blob.core.windows.net/users/{ImageId}";
        
        [Display(Name = "Rol")]
        public UserType UserType { get; set; }
        
        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<ServiceAccess> ServiceAccesses { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<PlanInscription> PlanInscriptions { get; set; }

        public ICollection<EventInscription> EventInscriptions { get; set; }
    }
}
