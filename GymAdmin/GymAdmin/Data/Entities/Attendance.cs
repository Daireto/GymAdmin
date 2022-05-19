namespace GymAdmin.Data.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        public User User { get; set; }

        public DateTime AttendanceDate { get; set; }
    }
}
