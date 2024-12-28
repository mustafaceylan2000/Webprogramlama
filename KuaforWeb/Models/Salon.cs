using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models
{
    public class Salon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        public int NumberOfRows { get; set; }

        public int SeatsPerRow { get; set; }

        // Navigation property to link to seats
        public virtual ICollection<Seat> Seats { get; set; }
    }
}