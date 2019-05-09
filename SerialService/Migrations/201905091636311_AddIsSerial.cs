namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsSerial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VideoMaterials", "IsSerial", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VideoMaterials", "IsSerial");
        }
    }
}
