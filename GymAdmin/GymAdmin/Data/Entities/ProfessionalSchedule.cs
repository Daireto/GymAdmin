using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Data.Entities
{
    public class ProfessionalSchedule
    {
        public int Id { get; set; }

        public Professional Professional { get; set; }

        public Schedule Schedule { get; set; }
    }
}
