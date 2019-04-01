namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using System.Linq;
    using Entities;
    using Context;

    public class GenreRepository : IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Genre> GetAllEntities()
        {
            return this.DB.Genres.ToEntityList();
        }

        public Genre GetEntity(int id)
        {
            return this.DB.Genres.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Genre entity)
        {
            if (entity == null)
                return false;

            this.DB.Genres.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Genre entity)
        {
            if (entity == null)
                return false;

            Genre cache = this.DB.Genres.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Genres.Remove(this.DB.Genres.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Genre entity)
        {
            this.DB.Genres.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;

    }
}
