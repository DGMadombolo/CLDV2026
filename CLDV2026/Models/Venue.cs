using System.ComponentModel.DataAnnotations;

namespace CLDV2026.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();
    }
}