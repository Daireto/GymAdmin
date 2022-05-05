﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace GymAdmin.Models
{
    public class AddEventViewModel
    {
        public int? Id { get; set; }
        [Display(Name = "nombre del evento")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "nombre del Director")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string DirectorUserName { get; set; }
        public IEnumerable<SelectListItem> Directors { get; set; }
        public DateTime WorkTime { get; set; }
    }
}