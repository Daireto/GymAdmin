using GymAdmin.Common;
using GymAdmin.Data.Entities;

namespace GymAdmin.Models
{
    public class EventsViewModel
    {
        public PaginatedList<Event> Events { get; set; }
    }
}
