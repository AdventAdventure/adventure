using System.ComponentModel.DataAnnotations.Schema;

namespace Adventure.Models
{
    [Table("Response")]
    public class Response
    {
        public virtual int ResponseId { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Tweet { get; set; }
        public virtual string TweetId { get; set; }
        public virtual int ChallengeId { get; set; }
    }
}