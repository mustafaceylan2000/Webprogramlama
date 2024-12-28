using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaforWeb.Models
{
    public class Randevu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Randevu Number")]
        public string RandevuNumber { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Finish Time")]
        public DateTime FinishTime { get; set; }

        // Stored Flight Duration
        [Display(Name = "Randevu Duration")]
        public TimeSpan RandevuDuration { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        // Flight Status (e.g., On-Time, Delayed, Cancelled)
        [Display(Name = "Randevu Status")]
        public string  RandevuStatus { get; set; }

        // Foreign Key - Airplane
        [Required]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        // Navigation property
        [ForeignKey("SalonId")]
        public virtual Salon Salon { get; set; }
    }
}
