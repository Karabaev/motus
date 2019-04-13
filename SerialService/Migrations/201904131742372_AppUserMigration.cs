namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserMigration : DbMigration
    {
        public override void Up()
        {
			DateTime defDateTime = new DateTime(2000, 1, 1);

			AddColumn("dbo.AspNetUsers", "RegisterDateTime", c => c.DateTime(nullable: false, defaultValue: defDateTime));
            AddColumn("dbo.AspNetUsers", "ChangeDateTime", c => c.DateTime(nullable: false, defaultValue: defDateTime));
            AddColumn("dbo.AspNetUsers", "LastAuthorizationDateTime", c => c.DateTime(nullable: false, defaultValue: defDateTime));
            AddColumn("dbo.AspNetUsers", "LastConfirmationKey", c => c.String());
            DropColumn("dbo.AspNetUsers", "PublicName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PublicName", c => c.String());
            DropColumn("dbo.AspNetUsers", "LastConfirmationKey");
            DropColumn("dbo.AspNetUsers", "LastAuthorizationDateTime");
            DropColumn("dbo.AspNetUsers", "ChangeDateTime");
            DropColumn("dbo.AspNetUsers", "RegisterDateTime");
        }
    }
}
