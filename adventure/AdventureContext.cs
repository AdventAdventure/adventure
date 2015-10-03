using System.Data.Entity;
using Adventure.Models;

namespace Adventure
{
    public class AdventureContext : DbContext
    {
        public DbSet<Day> Days { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
    }
}