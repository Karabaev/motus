namespace SerialService.ViewModels.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddCommentViewModel
    {
        [Required ]
        [StringLength(350, MinimumLength = 1)]
        public string Text { get; set; }
        public int? ParentID { get; set; }
        [Required]
        public int VideoMaterialID { get; set; }
    }
}