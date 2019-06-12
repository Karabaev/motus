namespace SerialService.DAL.Entities
{
    using System;
    using System.Collections.Generic;

    public class Comment : IBaseEntity
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string AuthorID { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public int ParentID { get; set; }
        public Comment Parent { get; set; }
        public List<Comment> DependentComments { get; set; }
        public List<CommentMark> Marks { get; set; }

        public bool Alike(IBaseEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}