namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class TranslationRepository : ITranslationRepository
    {
        public TranslationRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Translation> GetAllEntities()
        {
            return this.db.Translations.ToEntityList();
        }

        public Translation GetEntity(int id)
        {
            return this.db.Translations.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Translation entity)
        {
            if (entity == null)
                return false;

            this.db.Translations.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Translation entity)
        {
            if (entity == null)
                return false;

            Translation cache = this.db.Translations.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.Translations.Remove(this.db.Translations.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Translation entity)
        {
            this.db.Translations.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
