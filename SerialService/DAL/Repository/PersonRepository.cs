namespace SerialService.DAL.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class PersonRepository : IPersonRepository
    {
        public PersonRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Person> GetAllEntities()
        {
            return this.db.People.ToEntityList();
        }

        public Person GetEntity(int id)
        {
            return this.db.People.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Person entity)
        {
            if (entity == null)
                return false;

            this.db.People.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Person entity)
        {
            if (entity == null)
                return false;

            Person cache = this.db.People.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.People.Remove(this.db.People.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Person entity)
        {
            this.db.People.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}