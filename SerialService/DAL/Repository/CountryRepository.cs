namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using Entities;
    using Context;

    public class CountryRepository : ICountryRepository
    {
        public CountryRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Country> GetAllEntities()
        {
            return this.db.Countries.ToEntityList();
        }

        public Country GetEntity(int id)
        {
            return this.db.Countries.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Country entity)
        {
            if (entity == null)
                return false;

            this.db.Countries.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Country entity)
        {
            if (entity == null)
                return false;

            Country cache = this.db.Countries.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.Countries.Remove(this.db.Countries.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Country entity)
        {
            this.db.Countries.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
