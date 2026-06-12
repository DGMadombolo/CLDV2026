using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV2026.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int VenueId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [ForeignKey("EventTypeId")]
        public EventType? EventType { get; set; }
        public int? EventTypeId { get; set; }
        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}