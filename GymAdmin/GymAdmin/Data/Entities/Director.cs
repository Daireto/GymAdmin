using GymAdmin.Enums;

namespace GymAdmin.Data.Entities
{
    public class Director
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<Event> Events { get; set; }

        public int EventsNumber => Events == null ? 0 : Events.Count;
    }
}
