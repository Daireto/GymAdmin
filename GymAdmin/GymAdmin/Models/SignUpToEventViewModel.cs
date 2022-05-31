using GymAdmin.Enums;

namespace GymAdmin.Models
{
    public class SignUpToEventViewModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string DirectorFullName { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan FinishHour { get; set; }
        public EventType EventType { get; set; }
        public string Description { get; set; }
    }
}
