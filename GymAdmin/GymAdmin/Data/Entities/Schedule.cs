namespace GymAdmin.Data.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        public string Day { get; set; }

        public TimeSpan StartHour { get; set; }

        public TimeSpan EndHour { get; set; }

        public bool Avialable { get; set; }
        
        public UserProfessional userProfessional { get; set; }
    }
}
