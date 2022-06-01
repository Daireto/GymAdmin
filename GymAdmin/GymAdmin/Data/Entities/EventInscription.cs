using GymAdmin.Enums;
namespace GymAdmin.Data.Entities
{
    public class EventInscription
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Event Event { get; set; }

        public DateTime InscriptionDate { get; set; }

        public EventStatus EventStatus { get; set; }
    }
}
