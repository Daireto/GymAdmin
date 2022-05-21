using GymAdmin.Enums;
namespace GymAdmin.Data.Entities
{
    public class EventIncriptions
    {
        public int Id { get; set; }

        public User User { get; set; }
        public DateTime InscriptionDate { get; set; }
        public Event  Event { get; set; }

        public EventsStatus EventsStatus { get; set; }

    }
}
