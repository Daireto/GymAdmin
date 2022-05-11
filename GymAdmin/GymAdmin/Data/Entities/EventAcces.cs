namespace GymAdmin.Data.Entities
{
    public class EventAcces
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Service Service { get; set; }

        public DayOfWeek? AccessDate { get; set; }

        public DateTime? AccessHour { get; set; }


    }
}
