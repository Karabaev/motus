namespace SerialService.DAL.Context
{
    using System.Data.Entity;
    using Entities;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<VideoMark> VideoMarks { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<VideoMaterial> VideoMaterials { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SerialSeason> SerialSeasons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Genres).WithMany(g => g.VideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Themes).WithMany(t => t.VideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Countries).WithMany(t => t.VideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Actors).WithMany(t => t.ActorVideoMaterials).Map(a=>a.ToTable("ActorsVideoMaterials"));
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.FilmMakers).WithMany(t => t.FilmMakerVideoMaterials).Map(r=>r.ToTable("FilmMakersVideoMaterials"));
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Pictures).WithRequired(ss => ss.VideoMaterial);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Actors).WithMany(t => t.ActorVideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.FilmMakers).WithMany(t => t.FilmMakerVideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.Pictures).WithOptional(ss => ss.VideoMaterial);
            modelBuilder.Entity<VideoMaterial>().HasOptional(vmi => vmi.Author).WithMany(p => p.AddedVideoMaterials).WillCascadeOnDelete(false);
            modelBuilder.Entity<VideoMaterial>().HasMany(vm => vm.SubscribedUsers).WithMany(u => u.SubscribedVideoMaterials);
            modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.VideoMarks).WithRequired(p => p.VideoMaterial).WillCascadeOnDelete(false);
            modelBuilder.Entity<VideoMark>().HasOptional(vm => vm.Author).WithMany(u => u.VideoMarks);
            modelBuilder.Entity<Translation>().HasMany(trn => trn.SerialSeasons).WithRequired(s=>s.Translation);
			modelBuilder.Entity<VideoMaterial>().HasMany(vmi => vmi.SerialSeasons).WithRequired(s => s.VideoMaterial);

			modelBuilder.Entity<Translation>().Property(t => t.Name).IsRequired();
			modelBuilder.Entity<ApplicationUser>().Property(a => a.RegisterDateTime).IsRequired();
			modelBuilder.Entity<ApplicationUser>().Property(a => a.LastAuthorizationDateTime).IsRequired();
			modelBuilder.Entity<ApplicationUser>().Property(a => a.ChangeDateTime).IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}