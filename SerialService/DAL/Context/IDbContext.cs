namespace SerialService.DAL.Context
{
    using System;
    using System.Data.Entity;
    using Entities;
    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IDbContext : IDisposable
    {
        IDbSet<IdentityRole> Roles { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<CommentMark> CommentMarks { get; set; }
        DbSet<Picture> Pictures { get; set; }
        DbSet<Genre> Genres { get; set; }
        DbSet<VideoMark> VideoMarks { get; set; }
        DbSet<Theme> Themes { get; set; }
        DbSet<Translation> Translations { get; set; }
        DbSet<VideoMaterial> VideoMaterials { get; set; }
        DbSet<Person> People { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<SerialSeason> SerialSeasons { get; set; }
        DbSet<VideoMaterialViewsByUsers> VideoMaterialViewsByUsers { get; set; }
        DbSet<UserParam> UserParams { get; set; }

        int SaveChanges();
    }
}
