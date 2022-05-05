﻿using GymAdmin.Enums;

namespace GymAdmin.Data.Entities
{
    public class Director
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ProfessionalType ProfessionalType { get; set; }

        public ICollection<Event> Events { get; set; }

        public int EventsNumber => Events.Count;

        public Schedule Schedule { get; set; }
    }
}
