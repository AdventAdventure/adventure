namespace Adventure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScreenName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "ScreenName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "ScreenName");
        }
    }
}
