using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adventure.Models
{
    [Table("Badge")]
    public class Badge
    {
        public int BadgeId { get; set; }
        public string Name { get; set; }
        public /*BadgeCodes*/ string Code { get; set; }
    }

    public enum BadgeCodes
    {
        FirstParticipation,
        Image,
        Audio,
        Video,
        Streak3,
        Streak6,
        Streak12,
        Streak18,
        Streak24
    }
}