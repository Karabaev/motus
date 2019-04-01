namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class PictureRepository : IPictureRepository
    {
        public PictureRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Picture> GetAllEntities()
        {
            return this.DB.Pictures.ToEntityList();
        }

        public Picture GetEntity(int id)
        {
            return this.DB.Pictures.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Picture entity)
        {
            if (entity == null)
                return false;

            this.DB.Pictures.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Picture entity)
        {
            if (entity == null)
                return false;

            Picture cache = this.DB.Pictures.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Pictures.Remove(this.DB.Pictures.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Picture entity)
        {
            this.DB.Pictures.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
