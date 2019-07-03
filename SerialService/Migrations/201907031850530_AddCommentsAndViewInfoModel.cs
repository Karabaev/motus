namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentsAndViewInfoModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentMarks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserIP = c.String(),
                        Value = c.Boolean(nullable: false),
                        CommentID = c.Int(nullable: false),
                        AuthorID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Comments", t => t.CommentID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorID)
                .Index(t => t.CommentID)
                .Index(t => t.AuthorID);
            
            CreateTable(
                "dbo.VideoMaterialViewsByUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        UserIP = c.String(),
                        VideoMaterialID = c.Int(nullable: false),
                        EndTimeOfLastView = c.Int(nullable: false),
                        SerialSeasonID = c.Int(),
                        EpisodeNumber = c.Int(),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SerialSeasons", t => t.SerialSeasonID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterialID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.VideoMaterialID)
                .Index(t => t.SerialSeasonID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        AuthorID = c.String(nullable: false, maxLength: 128),
                        ParentID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorID, cascadeDelete: true)
                .ForeignKey("dbo.Comments", t => t.ParentID)
                .Index(t => t.AuthorID)
                .Index(t => t.ParentID);
            
            AddColumn("dbo.VideoMaterials", "IframeUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentMarks", "AuthorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentMarks", "CommentID", "dbo.Comments");
            DropForeignKey("dbo.Comments", "ParentID", "dbo.Comments");
            DropForeignKey("dbo.Comments", "AuthorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoMaterialViewsByUsers", "VideoMaterialID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialViewsByUsers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoMaterialViewsByUsers", "SerialSeasonID", "dbo.SerialSeasons");
            DropIndex("dbo.Comments", new[] { "ParentID" });
            DropIndex("dbo.Comments", new[] { "AuthorID" });
            DropIndex("dbo.VideoMaterialViewsByUsers", new[] { "SerialSeasonID" });
            DropIndex("dbo.VideoMaterialViewsByUsers", new[] { "VideoMaterialID" });
            DropIndex("dbo.VideoMaterialViewsByUsers", new[] { "UserID" });
            DropIndex("dbo.CommentMarks", new[] { "AuthorID" });
            DropIndex("dbo.CommentMarks", new[] { "CommentID" });
            DropColumn("dbo.VideoMaterials", "IframeUrl");
            DropTable("dbo.Comments");
            DropTable("dbo.VideoMaterialViewsByUsers");
            DropTable("dbo.CommentMarks");
        }
    }
}
