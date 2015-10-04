using System.Data.Entity;
using Adventure.Models;

namespace Adventure
{
    public class AdventureContext : DbContext
    {
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            Database.SetInitializer( new MigrateDatabaseToLatestVersion<AdventureContext, Migrations.Configuration>() );
        }
    }
}