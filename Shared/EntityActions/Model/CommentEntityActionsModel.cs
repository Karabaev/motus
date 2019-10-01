namespace Shared.EntityActions.Model
{
    public class CommentEntityActionsModel
    {
        public int ID { get; set; }
        public string AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialTitle { get; set; }
        public string Text { get; set; }
        public int? ParentID { get; set; }
        public string ParentAuthorEmail { get; set; }
        public string ParentAuthorName { get; set; }
    }
}
