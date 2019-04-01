namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class ThemeRepository : IThemeRepository
    {
        public ThemeRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Theme> GetAllEntities()
        {
            return this.DB.Themes.ToEntityList();
        }

        public Theme GetEntity(int id)
        {
            return this.DB.Themes.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Theme entity)
        {
            if (entity == null)
                return false;

            this.DB.Themes.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Theme entity)
        {
            if (entity == null)
                return false;

            Theme cache = this.DB.Themes.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Themes.Remove(this.DB.Themes.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Theme entity)
        {
            this.DB.Themes.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
