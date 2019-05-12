namespace SerialService.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginCallbackViewModel
    {
        [Required]
        public string ExternalProviderName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ReturnUrl { get; set; }
    }
}