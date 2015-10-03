namespace Adventure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Challenge",
                c => new
                    {
                        ChallengeId = c.Int(nullable: false, identity: true),
                        ChallengeNumber = c.Int(nullable: false),
                        Name = c.String(),
                        Bonus = c.Boolean(nullable: false),
                        Value = c.Int(),
                    })
                .PrimaryKey(t => t.ChallengeId);
            
            CreateTable(
                "dbo.Day",
                c => new
                    {
                        DayId = c.Int(nullable: false, identity: true),
                        DayNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DayId);
            
            CreateTable(
                "dbo.Response",
                c => new
                    {
                        ResponseId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Tweet = c.String(),
                        TweetId = c.String(),
                        ChallengeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResponseId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        TwitterId = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User");
            DropTable("dbo.Response");
            DropTable("dbo.Day");
            DropTable("dbo.Challenge");
        }
    }
}
