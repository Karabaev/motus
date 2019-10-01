namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCommentNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsSubscribedOnComments", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsSubscribedOnComments");
        }
    }
}
