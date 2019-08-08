namespace SerialService.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    public class VoteForCommentViewModel
    {
        [Required]
        public int CommentID { get; set; }
        [Required]
        public bool Value { get; set; }
    }
}