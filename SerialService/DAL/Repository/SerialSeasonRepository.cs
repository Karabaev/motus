namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class SerialSeasonRepository : ISerialSeasonRepository
    {
        public SerialSeasonRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<SerialSeason> GetAllEntities()
        {
            return this.DB.SerialSeasons.ToEntityList();
        }

        public SerialSeason GetEntity(int id)
        {
            return this.DB.SerialSeasons.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(SerialSeason entity)
        {
            if (entity == null)
                return false;

            this.DB.SerialSeasons.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(SerialSeason entity)
        {
            if (entity == null)
                return false;

            SerialSeason cache = this.DB.SerialSeasons.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.SerialSeasons.Remove(this.DB.SerialSeasons.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(SerialSeason entity)
        {
            this.DB.SerialSeasons.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
