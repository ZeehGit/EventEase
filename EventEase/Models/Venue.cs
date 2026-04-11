using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Venue Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Capacity { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageURL { get; set; }

        public string? Description { get; set; }

        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
    }
}