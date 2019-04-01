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
        public PersonRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Person> GetAllEntities()
        {
            return this.DB.People.ToEntityList();
        }

        public Person GetEntity(int id)
        {
            return this.DB.People.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Person entity)
        {
            if (entity == null)
                return false;

            this.DB.People.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Person entity)
        {
            if (entity == null)
                return false;

            Person cache = this.DB.People.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.People.Remove(this.DB.People.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Person entity)
        {
            this.DB.People.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}