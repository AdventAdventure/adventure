using System.ComponentModel.DataAnnotations.Schema;

namespace Adventure.Models
{
    [Table("Challenge")]
    public class Challenge
    {
        public virtual int ChallengeId { get; set; }
        public virtual int ChallengeNumber { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Bonus { get; set; }
        public virtual int? Value { get; set; }
        public virtual string Type { get; set; }
        public virtual string InfoResponse { get; set; }
        public string InfoTweet { get; set; }
    }
}