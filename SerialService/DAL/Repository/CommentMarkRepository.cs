namespace SerialService.DAL.Repository
{
    using System.Linq;
    using Infrastructure.Core;
    using Infrastructure.Core.Extensions;
    using Entities;
    using Context;

    public class CommentMarkRepository : ICommentMarkRepository
    {
        public CommentMarkRepository(IDbContext context)
        {
            this.db = context;
        }

        public EntityList<CommentMark> GetAllEntities()
        {
             return this.db.CommentMarks.ToEntityList();
        }

        public CommentMark GetEntity(int id)
        {
            return this.db.CommentMarks.FirstOrDefault(p => p.ID == id);
        }

        public bool AddEntity(CommentMark entity)
        {
            if (entity == null)
                return false;

            this.db.CommentMarks.Add(entity);
            return this.SaveChanges();
        }

        public bool UpdateEntity(CommentMark entity)
        {
            if (entity == null)
                return false;

            CommentMark cache = this.db.CommentMarks.FirstOrDefault(p => p.ID == entity.ID);

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
            this.db.CommentMarks.Remove(this.db.CommentMarks.FirstOrDefault(e => e.ID == id));
            return this.SaveChanges();
        }

        public bool RemoveEntity(CommentMark entity)
        {
            this.db.CommentMarks.Remove(entity);
            return this.SaveChanges();
        }

        public bool SaveChanges()
        {
             return this.db.SaveChanges() > 0;
        }

        private readonly IDbContext db;
    }
}
