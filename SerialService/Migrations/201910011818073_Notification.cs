namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserParams",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserParams", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.UserParams", new[] { "UserID" });
            DropTable("dbo.UserParams");
        }
    }
}
