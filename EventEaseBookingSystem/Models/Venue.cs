



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //  Important to add
using Microsoft.AspNetCore.Http; // Important to add
 
namespace EventEaseBookingSystem.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        [Required]
        public string? VenueName { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        // This stays — to store the URL of the image uploaded
        public string? ImageUrl { get; set; }

        // Add this new one — only for uploading from the Create/Edit form
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
