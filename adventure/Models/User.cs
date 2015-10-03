using System.ComponentModel.DataAnnotations.Schema;

namespace Adventure.Models
{
    [Table("User")]
    public class User
    {
        public virtual int UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string TwitterId { get; set; }
        public virtual string ScreenName { get; set; }
    }
}