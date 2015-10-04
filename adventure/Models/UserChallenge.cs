using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adventure.Models
{
    [Table("UserChallenge")]
    public class UserChallenge
    {
        public int UserChallengeId { get; set; }
        public int UserId { get; set; }
        public int ChallengeId { get; set; }
        public bool IsComplete { get; set; }
    }
    
}