namespace SerialService.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    public class RemoveCommentViewModel
    {
        [Required]
        public int? CommentID { get; set; }
    }
}