namespace SerialService.ViewModels.User
{
    using System;

    public class ShowCommentViewModel
    {
        public int ID { get; set; }
        public ShowCommentParentViewModel Parent { get; set; }
        public int HierarchyLevel { get; set; }
        public string AuthorID { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public string Text { get; set; }
        public int PositiveVoteCount { get; set; }
        public int NegativeVoteCount { get; set; }
        public DateTime AddDateTime { get; set; }
    }
}