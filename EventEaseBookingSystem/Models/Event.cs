using EventEaseBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
 
namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        public int EventID { get; set; }

        [Required]
        public string? EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        public int? VenueID { get; set; }
        public Venue? Venue { get; set; }

        

    }
}