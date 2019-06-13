namespace SerialService.DAL.Entities
{
    using System;

    public class CommentMark : IBaseEntity
    {
        public int ID { get; set; }
        public string UserIP { get; set; }
        public bool Value { get; set; }
        public int CommentID { get; set; }
        public virtual Comment Comment { get; set; }
        public string AuthorID { get; set; }
        public ApplicationUser Author { get; set; }

        /// <summary>
        /// Проверяет на равенство объекты по главным свойствам. (VideoMaterialID и UserIP)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Alike(IBaseEntity entity)
        {
            CommentMark mark = entity as CommentMark;

            if (mark == null)
                return false;

            return this.CommentID == mark.CommentID && (this.AuthorID == mark.AuthorID || this.UserIP == mark.UserIP);
        }
    }
}