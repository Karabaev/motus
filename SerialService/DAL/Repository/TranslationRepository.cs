namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Entities;
    using Context;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;

    public class TranslationRepository : ITranslationRepository
    {
        public TranslationRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Translation> GetAllEntities()
        {
            return this.DB.Translations.ToEntityList();
        }

        public Translation GetEntity(int id)
        {
            return this.DB.Translations.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Translation entity)
        {
            if (entity == null)
                return false;

            this.DB.Translations.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Translation entity)
        {
            if (entity == null)
                return false;

            Translation cache = this.DB.Translations.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Translations.Remove(this.DB.Translations.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Translation entity)
        {
            this.DB.Translations.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
