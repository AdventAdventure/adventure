using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adventure.Models
{
    [Table("UserBadge")]
    public class UserBadge
    {
        public int UserId { get; set; }
        public int BadgeId { get; set; }
    }
}