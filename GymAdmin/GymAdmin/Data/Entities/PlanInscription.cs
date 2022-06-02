using GymAdmin.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymAdmin.Data.Entities
{
    public class PlanInscription
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Plan Plan { get; set; }

        public PlanStatus PlanStatus { get; set; }

        public DateTime InscriptionDate { get; set; }

        public DateTime ActivationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Duration { get; set; }

        public int RemainingDays { get; set; }

        public double Discount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }
    }
}
