namespace GymAdmin.Models
{
    public class DashboardViewModel
    {
        public int Users { get; set; }
        public int Professionals { get; set; }
        public int Directors { get; set; }
        public int PendingContracts { get; set; }
        public decimal IncomeBySuscriptions { get; set; }
        public decimal IncomeByContracts { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
