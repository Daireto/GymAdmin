using GymAdmin.Enums;

namespace GymAdmin.Data.Entities
{
    public class Professional
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ProfessionalType ProfessionalType { get; set; }

        public ICollection<Service> Services { get; set; }

        public int ServicesNumber => Services.Count;

        public Schedule Schedule { get; set; }
    }
}
