namespace Adventure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChallenges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserChallenge",
                c => new
                    {
                        UserChallengeId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ChallengeId = c.Int(nullable: false),
                        IsComplete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserChallengeId);
            
            AddColumn("dbo.Challenge", "InfoResponse", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Challenge", "InfoResponse");
            DropTable("dbo.UserChallenge");
        }
    }
}
