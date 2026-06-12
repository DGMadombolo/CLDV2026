using System.ComponentModel.DataAnnotations;

namespace CLDV2026.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }
        public string Name { get; set; }
    }
}
