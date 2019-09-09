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
        public ThemeRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Theme> GetAllEntities()
        {
            return this.db.Themes.ToEntityList();
        }

        public Theme GetEntity(int id)
        {
            return this.db.Themes.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Theme entity)
        {
            if (entity == null)
                return false;

            this.db.Themes.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Theme entity)
        {
            if (entity == null)
                return false;

            Theme cache = this.db.Themes.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.Themes.Remove(this.db.Themes.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Theme entity)
        {
            this.db.Themes.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
