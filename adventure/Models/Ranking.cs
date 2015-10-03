using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adventure.Models
{
    public class Ranking
    {
        public IEnumerable<Position> Positions { get; set; }
    }

    public class Position
    {
        public string UserName { get; set; }
        public int? Points { get; set; }
    }
}