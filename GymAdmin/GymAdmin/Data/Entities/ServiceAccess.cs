using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Data.Entities
{
    public class ServiceAccess
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Service Service { get; set; }

        public DayOfWeek AccessDate { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan AccessHour { get; set; }

        public double Discount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        public ServiceStatus ServiceStatus { get; set; } //TODO: Add this to events
    }
}
