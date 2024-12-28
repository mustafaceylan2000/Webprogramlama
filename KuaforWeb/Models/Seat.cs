using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Seat Code")]
        public string SeatCode { get; set; }
        // e.g., Economy, Business

        [Required]
        [Display(Name = "Availability")]
        public bool IsAvailable { get; set; } = true; // By default, a seat is available

        // Foreign Key - Airplane
        [Required]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        // Navigation property
        [ForeignKey("SalonId")]
        public virtual Salon Salon { get; set; }
    }
}
