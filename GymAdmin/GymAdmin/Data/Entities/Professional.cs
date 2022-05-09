using GymAdmin.Enums;

namespace GymAdmin.Data.Entities
{
    public class Professional
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ProfessionalType ProfessionalType { get; set; }

        public Service Service { get; set; }

        public ICollection<ProfessionalSchedule> ProfessionalSchedules { get; set; }

        public int ProfessionalSchedulesNumber => ProfessionalSchedules == null ? 0 : ProfessionalSchedules.Count;
    }
}
