namespace SerialService.DAL.Repository
{
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using System.Linq;
    using System.Data.Entity;

    public class VideoMaterialViewsByUsersRepository : IVideoMaterialViewsByUsersRepository
    {
        public VideoMaterialViewsByUsersRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<VideoMaterialViewsByUsers> GetAllEntities()
        {
             return this.db.VideoMaterialViewsByUsers.ToEntityList();
        }

        public VideoMaterialViewsByUsers GetEntity(int id)
        {
            return this.db.VideoMaterialViewsByUsers.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(VideoMaterialViewsByUsers entity)
        {
            if (entity == null)
                return false;

            this.db.VideoMaterialViewsByUsers.Add(entity);
            return this.SaveChanges();
        }
        /// <summary>
        /// Обновить материал
        /// </summary>
        public bool UpdateEntity(VideoMaterialViewsByUsers entity)
        {
            if (entity == null)
                return false;

            VideoMaterialViewsByUsers cache = this.GetEntity(entity.ID);

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
            this.db.VideoMaterialViewsByUsers.Remove(this.db.VideoMaterialViewsByUsers.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(VideoMaterialViewsByUsers enity)
        {
            this.db.VideoMaterialViewsByUsers.Remove(enity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
