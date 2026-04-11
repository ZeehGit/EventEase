using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Required]
        [Display(Name = "Booking Reference")]
        public string? UniqueBookingRef { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public int VenueID { get; set; }

        [Required]
        [Display(Name = "Event")]
        public int EventID { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        // Navigation properties
        public Venue? Venue { get; set; }
        public Event? Event { get; set; }
    }
}