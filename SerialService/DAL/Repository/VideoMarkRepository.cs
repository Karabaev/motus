namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class VideoMarkRepository : IVideoMarkRepository
    {
        public VideoMarkRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<VideoMark> GetAllEntities()
        {
            return this.db.VideoMarks.ToEntityList();
        }

        public VideoMark GetEntity(int id)
        {
            return this.db.VideoMarks.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(VideoMark entity)
        {
            if (entity == null)
                return false;

            this.db.VideoMarks.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(VideoMark entity)
        {
            if (entity == null)
                return false;

            VideoMark cache = this.db.VideoMarks.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.VideoMarks.Remove(this.db.VideoMarks.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(VideoMark entity)
        {
            this.db.VideoMarks.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
