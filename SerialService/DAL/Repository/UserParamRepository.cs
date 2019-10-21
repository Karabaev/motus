namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using Entities;
    using Context;

    public class UserParamRepository : IUserParamRepository
    {
        public UserParamRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<UserParam> GetAllEntities()
        {
            return this.db.UserParams.ToEntityList();
        }

        public UserParam GetEntity(int id)
        {
            return this.db.UserParams.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(UserParam entity)
        {
            if (entity == null)
                return false;

            this.db.UserParams.Add(entity);
            bool result = this.SaveChanges();
            return result;
        }

        public bool UpdateEntity(UserParam entity)
        {
            if (entity == null)
                return false;

            UserParam cache = this.db.UserParams.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.UserParams.Remove(this.db.UserParams.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(UserParam entity)
        {
            this.db.UserParams.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
