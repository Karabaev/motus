namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCommentDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "IsArchived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "PositiveVoteCount", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "NegativeVoteCount", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "HierarchyLevel", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Comments", "VideoMaterialID", c => c.Int(nullable: false));
            CreateIndex("dbo.Comments", "VideoMaterialID");
            AddForeignKey("dbo.Comments", "VideoMaterialID", "dbo.VideoMaterials", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "VideoMaterialID", "dbo.VideoMaterials");
            DropIndex("dbo.Comments", new[] { "VideoMaterialID" });
            DropColumn("dbo.Comments", "VideoMaterialID");
            DropColumn("dbo.Comments", "AddDateTime");
            DropColumn("dbo.Comments", "HierarchyLevel");
            DropColumn("dbo.Comments", "NegativeVoteCount");
            DropColumn("dbo.Comments", "PositiveVoteCount");
            DropColumn("dbo.Comments", "IsArchived");
        }
    }
}
