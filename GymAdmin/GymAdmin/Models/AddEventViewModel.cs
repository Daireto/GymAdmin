using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Models
{
    public class AddEventViewModel : EditEventViewModel
    {
        [Display(Name = "Foto principal")]
        public IFormFile? ImageFile { get; set; }

        public string DirectorUsername { get; set; }
    }
}
