namespace SerialService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.VideoMaterials",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        OriginalTitle = c.String(),
                        Text = c.String(),
                        Tagline = c.String(),
                        KinopoiskID = c.String(),
                        IDMB = c.Single(nullable: false),
                        KinopoiskRating = c.Single(),
                        Duration = c.Int(),
                        AuthorID = c.String(maxLength: 128),
                        UpdateDateTime = c.DateTime(),
                        AddDateTime = c.DateTime(),
                        MoonWalkAddDate = c.DateTime(),
                        ReleaseDate = c.Int(),
                        PositiveMarkCount = c.Int(nullable: false),
                        NegativeMarkCount = c.Int(nullable: false),
                        CheckStatus = c.Int(nullable: false),
                        WatchForUpdates = c.Boolean(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorID)
                .Index(t => t.AuthorID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        RegisterDateTime = c.DateTime(nullable: false),
                        ChangeDateTime = c.DateTime(nullable: false),
                        LastAuthorizationDateTime = c.DateTime(nullable: false),
                        Parole = c.String(),
                        AvatarURL = c.String(),
                        IsLocked = c.Boolean(nullable: false),
                        LastConfirmationKey = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.VideoMarks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserIP = c.String(),
                        Value = c.Boolean(nullable: false),
                        VideoMaterialID = c.Int(nullable: false),
                        AuthorID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorID)
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterialID)
                .Index(t => t.VideoMaterialID)
                .Index(t => t.AuthorID);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        URL = c.String(),
                        VideoMaterialID = c.Int(),
                        IsPoster = c.Boolean(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterialID)
                .Index(t => t.VideoMaterialID);
            
            CreateTable(
                "dbo.SerialSeasons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SeasonNumber = c.Int(nullable: false),
                        EpisodesCount = c.Int(),
                        LastEpisodeTime = c.DateTime(),
                        VideoMaterialID = c.Int(nullable: false),
                        TranslationID = c.Int(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Translations", t => t.TranslationID, cascadeDelete: true)
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterialID, cascadeDelete: true)
                .Index(t => t.VideoMaterialID)
                .Index(t => t.TranslationID);
            
            CreateTable(
                "dbo.Translations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Themes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.VideoMaterialPersons",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        Person_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.Person_ID })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_ID, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.Person_ID);
            
            CreateTable(
                "dbo.VideoMaterialCountries",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        Country_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.Country_ID })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_ID, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.Country_ID);
            
            CreateTable(
                "dbo.VideoMaterialPerson1",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        Person_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.Person_ID })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_ID, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.Person_ID);
            
            CreateTable(
                "dbo.VideoMaterialGenres",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        Genre_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.Genre_ID })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.Genres", t => t.Genre_ID, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.Genre_ID);
            
            CreateTable(
                "dbo.VideoMaterialApplicationUsers",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.ApplicationUser_Id })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.VideoMaterialThemes",
                c => new
                    {
                        VideoMaterial_ID = c.Int(nullable: false),
                        Theme_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoMaterial_ID, t.Theme_ID })
                .ForeignKey("dbo.VideoMaterials", t => t.VideoMaterial_ID, cascadeDelete: true)
                .ForeignKey("dbo.Themes", t => t.Theme_ID, cascadeDelete: true)
                .Index(t => t.VideoMaterial_ID)
                .Index(t => t.Theme_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.VideoMarks", "VideoMaterialID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialThemes", "Theme_ID", "dbo.Themes");
            DropForeignKey("dbo.VideoMaterialThemes", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoMaterialApplicationUsers", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropForeignKey("dbo.SerialSeasons", "VideoMaterialID", "dbo.VideoMaterials");
            DropForeignKey("dbo.SerialSeasons", "TranslationID", "dbo.Translations");
            DropForeignKey("dbo.Pictures", "VideoMaterialID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialGenres", "Genre_ID", "dbo.Genres");
            DropForeignKey("dbo.VideoMaterialGenres", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialPerson1", "Person_ID", "dbo.People");
            DropForeignKey("dbo.VideoMaterialPerson1", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterialCountries", "Country_ID", "dbo.Countries");
            DropForeignKey("dbo.VideoMaterialCountries", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropForeignKey("dbo.VideoMaterials", "AuthorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoMarks", "AuthorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoMaterialPersons", "Person_ID", "dbo.People");
            DropForeignKey("dbo.VideoMaterialPersons", "VideoMaterial_ID", "dbo.VideoMaterials");
            DropIndex("dbo.VideoMaterialThemes", new[] { "Theme_ID" });
            DropIndex("dbo.VideoMaterialThemes", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.VideoMaterialApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.VideoMaterialApplicationUsers", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.VideoMaterialGenres", new[] { "Genre_ID" });
            DropIndex("dbo.VideoMaterialGenres", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.VideoMaterialPerson1", new[] { "Person_ID" });
            DropIndex("dbo.VideoMaterialPerson1", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.VideoMaterialCountries", new[] { "Country_ID" });
            DropIndex("dbo.VideoMaterialCountries", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.VideoMaterialPersons", new[] { "Person_ID" });
            DropIndex("dbo.VideoMaterialPersons", new[] { "VideoMaterial_ID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SerialSeasons", new[] { "TranslationID" });
            DropIndex("dbo.SerialSeasons", new[] { "VideoMaterialID" });
            DropIndex("dbo.Pictures", new[] { "VideoMaterialID" });
            DropIndex("dbo.VideoMarks", new[] { "AuthorID" });
            DropIndex("dbo.VideoMarks", new[] { "VideoMaterialID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.VideoMaterials", new[] { "AuthorID" });
            DropTable("dbo.VideoMaterialThemes");
            DropTable("dbo.VideoMaterialApplicationUsers");
            DropTable("dbo.VideoMaterialGenres");
            DropTable("dbo.VideoMaterialPerson1");
            DropTable("dbo.VideoMaterialCountries");
            DropTable("dbo.VideoMaterialPersons");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Themes");
            DropTable("dbo.Translations");
            DropTable("dbo.SerialSeasons");
            DropTable("dbo.Pictures");
            DropTable("dbo.Genres");
            DropTable("dbo.VideoMarks");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.People");
            DropTable("dbo.VideoMaterials");
            DropTable("dbo.Countries");
        }
    }
}
