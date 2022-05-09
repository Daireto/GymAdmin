using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddProfessionalViewModel : AddUserViewModel
    {
        [Display(Name = "Profesión")]
        public ProfessionalType ProfessionalType { get; set; }
    }
}
