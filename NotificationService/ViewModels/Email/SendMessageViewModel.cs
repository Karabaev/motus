namespace NotificationService.ViewModels.Email
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SendMessageViewModel
    {
        [Required]
        public IEnumerable<string> Destinations { get; set; }
        [Required]
        public string Caption { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
