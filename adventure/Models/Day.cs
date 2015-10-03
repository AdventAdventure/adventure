using System.ComponentModel.DataAnnotations.Schema;

namespace Adventure.Models
{
    [Table("Day")]
    public class Day
    {
        public virtual int DayId { get; set; }
        public virtual int DayNumber { get; set; }
    }
}