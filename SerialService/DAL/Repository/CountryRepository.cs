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
        public CountryRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Country> GetAllEntities()
        {
            return this.DB.Countries.ToEntityList();
        }

        public Country GetEntity(int id)
        {
            return this.DB.Countries.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Country entity)
        {
            if (entity == null)
                return false;

            this.DB.Countries.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Country entity)
        {
            if (entity == null)
                return false;

            Country cache = this.DB.Countries.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Countries.Remove(this.DB.Countries.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Country entity)
        {
            this.DB.Countries.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
