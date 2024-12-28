using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "Is Purchased")]
        public bool IsPurchased { get; set; } = false;

        // Computed property for Purchase Deadline
        [Display(Name = "Purchase Deadline")]
        public DateTime? PurchaseDeadline
        {
            get
            {
                // Check if Flight is null before accessing its properties
                return Randevu == null ? (DateTime?)null : Randevu.StartTime.AddHours(-2);
            }
        }

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
