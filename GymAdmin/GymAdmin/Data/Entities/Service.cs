using System.ComponentModel.DataAnnotations;

namespace GymAdmin.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        public User User { get; set; }

        public ICollection<User> Professionals { get; set; } //the idea is to make a list of the professionals avialable on certain time 
                                                             //Remember to hitch the professionals 

    }
}
