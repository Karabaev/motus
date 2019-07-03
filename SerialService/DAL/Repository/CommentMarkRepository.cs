//namespace SerialService.DAL.Repository
//{
//    using System.Linq;
//    using Infrastructure.Core;
//    using Infrastructure.Core.Extensions;
//    using Entities;
//    using Context;

//    public class CommentMarkRepository : ICommentMarkRepository
//    {
//        public CommentMarkRepository(ApplicationDbContext context)
//        {
//            this.DB = context;
//        }

//        public EntityList<CommentMark> GetAllEntities()
//        {
//            throw new System.NotImplementedException();
//           // return this.DB.CommentMarks.ToEntityList();
//        }

//        public CommentMark GetEntity(int id)
//        {
//            throw new System.NotImplementedException();
//            //return this.DB.CommentMarks.FirstOrDefault(p => p.ID == id);
//        }

//        public bool AddEntity(CommentMark entity)
//        {
//            throw new System.NotImplementedException();
//            //if (entity == null)
//            //    return false;

//            //this.DB.CommentMarks.Add(entity);
//            //return this.SaveChanges();
//        }

//        public bool UpdateEntity(CommentMark entity)
//        {
//            throw new System.NotImplementedException();
//            //if (entity == null)
//            //    return false;

//            //CommentMark cache = this.DB.CommentMarks.FirstOrDefault(p => p.ID == entity.ID);

//            //if (cache == null)
//            //    return false;

//            //foreach (var item in entity.GetType().GetProperties())
//            //{
//            //    if (item.Name != "ID")
//            //        item.SetValue(cache, item.GetValue(entity));
//            //}

//            //return this.SaveChanges();
//        }

//        public bool RemoveEntity(int id)
//        {
//            throw new System.NotImplementedException();
//            //this.DB.CommentMarks.Remove(this.DB.CommentMarks.FirstOrDefault(e => e.ID == id));
//            //return this.SaveChanges();
//        }

//        public bool RemoveEntity(CommentMark entity)
//        {
//            throw new System.NotImplementedException();
//            //this.DB.CommentMarks.Remove(entity);
//            //return this.SaveChanges();
//        }

//        public bool SaveChanges()
//        {
//            throw new System.NotImplementedException();
//           // return this.DB.SaveChanges() > 0;
//        }

//        private readonly ApplicationDbContext DB;
//    }
//}
