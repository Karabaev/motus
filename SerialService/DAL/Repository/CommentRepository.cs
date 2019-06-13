namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using Entities;
    using Context;

    public class CommentRepository : ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context)
        {
            this.DB = context;
        }

        public EntityList<Comment> GetAllEntities()
        {
            return this.DB.Comments.ToEntityList();
        }

        public Comment GetEntity(int id)
        {
            return this.DB.Comments.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(Comment entity)
        {
            if (entity == null)
                return false;

            this.DB.Comments.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(Comment entity)
        {
            if (entity == null)
                return false;

            Comment cache = this.DB.Comments.FirstOrDefault(p => p.ID == entity.ID);

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
            this.DB.Comments.Remove(this.DB.Comments.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(Comment entity)
        {
            this.DB.Comments.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return this.DB.SaveChanges() > 0;
        }

        private readonly ApplicationDbContext DB;
    }
}
