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
        public GenreRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Genre> GetAllEntities()
        {
            return this.db.Genres.ToEntityList();
        }

        public Genre GetEntity(int id)
        {
            return this.db.Genres.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Genre entity)
        {
            if (entity == null)
                return false;

            this.db.Genres.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Genre entity)
        {
            if (entity == null)
                return false;

            Genre cache = this.db.Genres.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.Genres.Remove(this.db.Genres.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Genre entity)
        {
            this.db.Genres.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;

    }
}
