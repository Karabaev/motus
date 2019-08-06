namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OneToManyCommentsRelationShip : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "ParentID", "dbo.Comments");
            AddColumn("dbo.Comments", "Comment_ID", c => c.Int());
            CreateIndex("dbo.Comments", "Comment_ID");
            AddForeignKey("dbo.Comments", "Comment_ID", "dbo.Comments", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Comment_ID", "dbo.Comments");
            DropIndex("dbo.Comments", new[] { "Comment_ID" });
            DropColumn("dbo.Comments", "Comment_ID");
            AddForeignKey("dbo.Comments", "ParentID", "dbo.Comments", "ID");
        }
    }
}
