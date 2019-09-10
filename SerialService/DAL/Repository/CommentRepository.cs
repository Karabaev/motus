namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using Entities;
    using Context;

    public class CommentRepository : ICommentRepository
    {
        public CommentRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<Comment> GetAllEntities()
        {
            return this.db.Comments.ToEntityList();
        }

        public Comment GetEntity(int id)
        {
            return this.db.Comments.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Comment entity)
        {
            if (entity == null)
                return false;

            this.db.Comments.Add(entity);
            bool result = this.SaveChanges();
            return result;
        }

        public bool UpdateEntity(Comment entity)
        {
            if (entity == null)
                return false;

            Comment cache = this.db.Comments.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.Comments.Remove(this.db.Comments.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Comment entity)
        {
            this.db.Comments.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
