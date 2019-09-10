namespace SerialService.DAL.Repository
{
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using System.Linq;
    using System.Data.Entity;

    public class VideoMaterialRepository : IVideoMaterialRepository
    {
        public VideoMaterialRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<VideoMaterial> GetAllEntities()
        {
            return this.db.VideoMaterials.ToEntityList();
        }

        public VideoMaterial GetEntity(int id)
        {
            return this.db.VideoMaterials.FirstOrDefault(p => p.ID == id);
        }
        /// <summary>
        /// Метод "неленивой закгрузки" материала
        /// </summary>
        public VideoMaterial GetEntityNotLazyLoad(int id)
        {
            ((ApplicationDbContext)this.db).Configuration.LazyLoadingEnabled = false;
            ((ApplicationDbContext)this.db).Configuration.ProxyCreationEnabled = false;
            var result = this.db.VideoMaterials
                .Include(vm => vm.Actors)
                .Include(vm => vm.FilmMakers)
                .Include(vm => vm.Countries)
                .Include(vm => vm.Author)
                .Include(vm => vm.Countries)
                .Include(vm => vm.Genres)
                .Include(vm => vm.Pictures)
                .Include(vm => vm.SerialSeasons)
                .Include(vm => vm.SubscribedUsers)
                .Include(vm => vm.Themes)
                .Include(vm => vm.VideoMarks)
                .FirstOrDefault(vm => vm.ID == id);
            return result;
        }

        public bool AddEntity(VideoMaterial entity)
        {
            if (entity == null)
                return false;

            this.db.VideoMaterials.Add(entity);
            return this.SaveChanges();
        }
        /// <summary>
        /// Обновить материал
        /// </summary>
        public bool UpdateEntity(VideoMaterial entity)
        {
            if (entity == null)
                return false;

            VideoMaterial cache = this.GetEntity(entity.ID);

            if (cache == null)
                return false;
            foreach (var item in entity.GetType().GetProperties())
            {
                if (item.Name != "ID")
                    item.SetValue(cache, item.GetValue(entity));
            }
            return this.SaveChanges();
        }

        public bool RemoveEntity(int id)
        {
            this.db.VideoMaterials.Remove(this.db.VideoMaterials.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(VideoMaterial enity)
        {
            this.db.VideoMaterials.Remove(enity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
