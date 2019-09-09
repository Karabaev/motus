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
        public SerialSeasonRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<SerialSeason> GetAllEntities()
        {
            return this.db.SerialSeasons.ToEntityList();
        }

        public SerialSeason GetEntity(int id)
        {
            return this.db.SerialSeasons.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(SerialSeason entity)
        {
            if (entity == null)
                return false;

            this.db.SerialSeasons.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(SerialSeason entity)
        {
            if (entity == null)
                return false;

            SerialSeason cache = this.db.SerialSeasons.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.SerialSeasons.Remove(this.db.SerialSeasons.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(SerialSeason entity)
        {
            this.db.SerialSeasons.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
