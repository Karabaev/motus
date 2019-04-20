namespace SerialService.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginFailureViewModel
    {
        [Required]
        public string ProviderDisplayName { get; set; }
        [Required]
        public string Errors { get; set; }
    }
}