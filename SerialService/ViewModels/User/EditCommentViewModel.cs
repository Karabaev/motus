namespace SerialService.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    public class EditCommentViewModel
    {
        [Required]
        public int CommentID { get; set; }
        [Required]
        public string NewText { get; set; }
    }
}