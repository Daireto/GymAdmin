using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Models
{
    public class AddImageViewModel
    {
        public int EventId { get; set; }

        [Display(Name = "Imagen")]
        public IFormFile? ImageFile { get; set; }
    }
}
