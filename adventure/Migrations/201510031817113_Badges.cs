namespace Adventure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Badges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Badge",
                c => new
                    {
                        BadgeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.BadgeId);
            
            CreateTable(
                "dbo.UserBadge",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        BadgeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.BadgeId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserBadge");
            DropTable("dbo.Badge");
        }
    }
}
