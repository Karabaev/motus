namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Comments", "HierarchyLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "HierarchyLevel", c => c.Int(nullable: false));
        }
    }
}
