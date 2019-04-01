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
        public VideoMarkRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<VideoMark> GetAllEntities()
        {
            return this.DB.VideoMarks.ToEntityList();
        }

        public VideoMark GetEntity(int id)
        {
            return this.DB.VideoMarks.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(VideoMark entity)
        {
            if (entity == null)
                return false;

            this.DB.VideoMarks.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(VideoMark entity)
        {
            if (entity == null)
                return false;

            VideoMark cache = this.DB.VideoMarks.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.VideoMarks.Remove(this.DB.VideoMarks.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(VideoMark entity)
        {
            this.DB.VideoMarks.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
