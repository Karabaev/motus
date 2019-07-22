namespace SerialService.DAL.Entities
{
    using System.Collections.Generic;

    public class Comment : IBaseEntity
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public bool IsArchived { get; set; }
        public string AuthorID { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public int ParentID { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual List<Comment> DependentComments { get; set; }
        public virtual List<CommentMark> Marks { get; set; }


        public bool Alike(IBaseEntity entity)
        {
            Comment comment = entity as Comment;

            if (comment == null)
                return false;

            return true;
        }
    }
}