using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace GymAdmin.Data.Entities
{
    public class EventAcces
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Event Event{ get; set; }
       
        public DayOfWeek? AccessDate { get; set; }

        public DateTime? AccessHour { get; set; }

        public EventsStatus EventStatus { get; set; }
    }
}
