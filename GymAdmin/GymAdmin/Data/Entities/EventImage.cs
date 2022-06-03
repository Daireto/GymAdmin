using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class EventImage
    {
        public int Id { get; set; }

        public Event Event { get; set; }

        [Display(Name = "Foto")]
        public Guid ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://gymadmin1.azurewebsites.net/images/noimage.png"
            : $"https://gymadmin1.blob.core.windows.net/events/{ImageId}";
    }
}
