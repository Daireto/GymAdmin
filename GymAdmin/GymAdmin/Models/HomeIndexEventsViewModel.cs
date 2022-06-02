using GymAdmin.Data.Entities;

namespace GymAdmin.Models
{
    public class HomeIndexEventsViewModel
    {
        public List<Event> MondayEvents { get; set; }
        public List<Event> TuesdayEvents { get; set; }
        public List<Event> WednesdayEvents { get; set; }
        public List<Event> ThursdayEvents { get; set; }
        public List<Event> FridayEvents { get; set; }
        public List<Event> SaturdayEvents { get; set; }
        public List<Event> SundayEvents { get; set; }
        public int MondayNumber { get; set; }
        public int TuesdayNumber { get; set; }
        public int WednesdayNumber { get; set; }
        public int ThursdayNumber { get; set; }
        public int FridayNumber { get; set; }
        public int SaturdayNumber { get; set; }
        public int SundayNumber { get; set; }
        public int RowsNumber { get; set; }
    }
}
