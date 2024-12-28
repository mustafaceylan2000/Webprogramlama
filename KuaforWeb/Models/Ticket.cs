using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ticket Number")]
        public string TicketNumber { get; set; } // A unique identifier for the ticket

        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        // Foreign Key - User
        [Required]
        public string UserId { get; set; }

        // Navigation property to User
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Foreign Key - Flight
        [Required]
        public int RandevuId { get; set; }

        // Navigation property to Flight
        [ForeignKey("RandevuId")]
        public virtual Randevu Randevu { get; set; }

        // Foreign Key - Seat
        [Required]
        public int SeatId { get; set; }

        // Navigation property to Seat
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }
    }
}
