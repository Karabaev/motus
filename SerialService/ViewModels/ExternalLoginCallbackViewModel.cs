namespace SerialService.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginCallbackViewModel
    {
        [Required]
        public string ExternalProviderName { get; set; }
    }
}